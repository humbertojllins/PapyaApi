using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Linq;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;

namespace papya_api.DataProvider
{
    public class PedidoDataProvider:IPedidoDataProvider
    {
        private readonly IConfiguration _configuration;
        ContaDetalheDataProvider dp;
        public PedidoDataProvider()
        {
            
        }

        public PedidoDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            dp = new ContaDetalheDataProvider(configuration);

        }

        public object AddPedido(IEnumerable<Pedido> listaPedido)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
               
                conexao.Open();

                int id_usuario_conta = listaPedido.First().id_usuario_conta;
                int fk_id_mesa = listaPedido.First().fk_id_mesa;


                var sql = "insert into pedido (fk_usuario_conta_id, fk_id_mesa)" +
                    " values(" + id_usuario_conta + "," + fk_id_mesa + ");";
                sql += " select last_insert_id();";

                int id_pedido = Convert.ToInt32(conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text));
                sql = "";
                foreach (Pedido item in listaPedido)
                {
                    sql += " insert into pedido_itens (id_pedido,fk_status_id, fk_itens_id, qtd,desconto)";
                    sql += " values(" + id_pedido + ", ";
                    sql += "1,";
                    sql += item.id_item + ",";
                    sql += item.qtd_item + ",";
                    sql += item.desconto + ");";
                }

                sql += " select id_conta from usuario_conta where id =" + id_usuario_conta;


                int id_conta = Convert.ToInt32(conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text));

                //Atualiza o total do pedido de acordo com os itens
                sql = "update pedido set total = (select sum(case when desconto > 0 then (i.valor-(i.valor * desconto)) else i.valor end * qtd) as valor_final from pedido_itens pi inner join itens i on pi.fk_itens_id = i.id where pi.id_pedido =" + id_pedido + ") where id =" + id_pedido + ";";
                //Atualiza o total da conta do usuario
                sql += " update usuario_conta set total = (select sum(total) from pedido where fk_usuario_conta_id ="+ id_usuario_conta + ") WHERE ID ="+ id_usuario_conta +";";
                //Atualiza o total da conta
                sql += " update conta set total = (select sum(total) from usuario_conta where id_conta ="+ id_conta + ") WHERE ID ="+  id_conta;

                //Atualiza os dados pela query
                conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);

                sql = " select i.id as codigo_item, i.titulo as item_titulo, i.descricao as item_descricao, i.valor as valor_item, pit.qtd as qtd_item, i.imagem, c.descricao as categoria ,u.nome, m.descricao as mesa ";
                sql +=" from pedido p";
                sql += " left join pedido_itens pit on p.id = pit.id_pedido";
                sql += " left join itens i on pit.fk_itens_id = i.id ";
                sql += " left join categoria c on c.id = i.fk_categoria_id ";
                sql += " left join usuario_conta uc on uc.id = p.fk_usuario_conta_id";
                sql += " left join usuario u on u.id = uc.id_usuario";
                sql += " left join conta con on con.id = uc.id_conta";
                sql += " left join mesa m on m.id = con.id_mesa";
                sql += " where p.id=" + id_pedido ;
                sql += " order by c.descricao ";

                IEnumerable<UltimoPedido> x = conexao.QueryAsync<UltimoPedido>(sql,
                null,
                commandType: System.Data.CommandType.Text).Result;

                //var ret = conexao.ExecuteAsync(sql, commandType: System.Data.CommandType.Text);
                return x;
                

            }
            //object listaPedidos = dp.GetDetalheContas(id_conta);
            //return dp.GetDetalheContas(id_conta);
        }


        public Task DeletePedido(int item)
        {
            throw new NotImplementedException();
        }

        public object GetPedidos(int idEstabelecimento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                 conexao.Open();

                var sql = "select " +
                    "i.id as codigo_item, " +
                    "i.descricao as item_titulo, " +
                    "i.descricao as item_descricao, " +
                    "i.valor as valor_item, " +
                    //"i.desconto as desconto_item, " +
                    "0 as desconto_item, " +
                    "i.imagem, " +
                    "c.descricao as categoria " +
                    "from itens i " +
                    "left join categoria c on c.id = i.fk_categoria_id " +
                    "where i.fk_id_estabelecimento = " + idEstabelecimento.ToString() +
                    " order by c.descricao;";

                IEnumerable<Cardapio> x = conexao.QueryAsync<Cardapio>(sql,
                null,
                commandType: System.Data.CommandType.Text).Result;
                string js= "[";
                var listaCateg = from a in x
                                          group a by a.categoria into newGroup
                                          orderby newGroup.Key
                                          select newGroup;
                int qtdCateg = 1;

                foreach (var item in listaCateg)
                {
                    js += "{";
                    js += "\"categoria\":";
                    //js += @"""" + item.Key + @"""itens:[";
                    //js += @"""" + item.Key + @""",";
                    js += "\"" + item.Key + "\",";
                    js += "\"itens\":[";
                    int loop = 0;
                    foreach (Cardapio c in x)
                    {
                        if (c.categoria == item.Key)
                        {
                            if (loop > 0)
                                js += ",";
                            js += " { ";
                            js += @"""codigo_item"":" + c.codigo_item + ",";
                            js += @"""titulo"":" + @"""" + c.item_titulo + @""",";
                            js += @"""descricao"":" + @"""" + c.item_descricao + @""",";
                            js += @"""valor"":" + c.valor_item + ",";
                            js += @"""desconto"":" + c.desconto + ",";
                            js += @"""imagem"":" + @"""" + c.imagem + @"""";
                            js += " }";
                            loop += 1;
                        }

                    }
                    js += " ]}";
                    loop = 0;
                    if (qtdCateg < listaCateg.Count())
                        js += ",";
                    qtdCateg += 1;
                }
                js += "]";

                //JArray array = new JArray();


                return js;
            }
        }

       
    }
}
