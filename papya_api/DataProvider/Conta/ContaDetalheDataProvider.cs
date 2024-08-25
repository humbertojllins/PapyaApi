using System.Collections.Generic;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Newtonsoft.Json;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;

namespace papya_api.DataProvider
{
    public class ContaDetalheDataProvider:IContaDetalheDataProvider
    {

        private readonly IConfiguration _configuration;

        public ContaDetalheDataProvider()
        {

        }
        public ContaDetalheDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /*
        public async Task<IEnumerable<object>> GetDetalheContas(int id_conta)
        {
                 using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "SELECT " +
                    "C.ID AS NUM_CONTA, " +
                    "U.ID AS USUARIO_ID," +
                "U.nome AS NOME_USUARIO," +
                "U.cpf AS CPF_USUARIO," +
                "M.DESCRICAO AS DESC_MESA," +
                "C.TOTAL AS VALOR_TOTAL_CONTA," +
                "UC.total AS VALOR_CONTA_USUARIO," +
                "CAT.DESCRICAO AS ITEM_CATEGORIA," +
                "I.DESCRICAO AS ITEM_DESCRICAO," +
                "PI.QTD AS ITEM_QTD, " +
                "I.VALOR AS ITEM_VALOR, " +
                "I.IMAGEM AS ITEM_IMAGEM " +

                "FROM CONTA C " +
                "LEFT JOIN STATUS_CONTA SC ON SC.id = C.id_status " +
                "LEFT JOIN USUARIO_CONTA UC ON C.id = UC.id_conta " +
                "LEFT JOIN USUARIO U ON UC.id_usuario = U.id " +
                "LEFT JOIN MESA M ON C.ID_MESA = M.ID " +
                "LEFT JOIN PEDIDO P ON  UC.ID = P.FK_USUARIO_CONTA_ID " +
                "LEFT JOIN PEDIDO_itens PI ON  P.ID = PI.ID_PEDIDO " +
                "LEFT JOIN itens I ON I.ID = PI.FK_itens_ID " +
                "LEFT JOIN CATEGORIA CAT ON CAT.ID = I.FK_CATEGORIA_ID " + 
                "WHERE C.ID=" + id_conta;



                return await conexao.QueryAsync<object>(sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }*/

        /// <summary>
        /// Reecupera os itens da conta do cliente
        /// </summary>
        /// <param name="id_conta"></param>
        /// <returns></returns>
        public object GetDetalheContas(int id_conta)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                 conexao.Open();

                string sql = "select " +
                "c.id as num_conta, " +
                "uc.id as id_usuario_conta," +
                "u.id as usuario_id," +
                "u.nome as nome_usuario," +
                "u.cpf as cpf_usuario," +
                "m.descricao as desc_mesa," +
                "c.total as valor_total_conta," +
                "uc.total as valor_conta_usuario," +
                "sc.descricao as status_conta, " +
                "sc2.descricao as status_conta_usuario, " +
                "cat.descricao as item_categoria," +
                "i.titulo as item_titulo," +
                "i.descricao as item_descricao," +
                "si.descricao as item_status, " +
                "pi.qtd as item_qtd, " +
                "i.valor as item_valor, " +
                "i.imagem as item_imagem, " +
                "pi.desconto as item_desconto, " +
                "pg.valor valor_pago_conta_usuario, " +
                "mpg.descricao usuario_meio_pagamento " +

                "from conta c " +
                "left join status_conta sc on sc.id = c.id_status " +
                "left join usuario_conta uc on c.id = uc.id_conta " +
                "left join status_conta sc2 on sc2.id = uc.status_conta_usuario " +
                "left join usuario u on uc.id_usuario = u.id " +
                "left join mesa m on c.id_mesa = m.id " +
                "left join pedido p on  uc.id = p.fk_usuario_conta_id " +
                "left join pedido_itens pi on  p.id = pi.id_pedido " +
                "left join itens i on i.id = pi.fk_itens_id " +
                "left join categoria cat on cat.id = i.fk_categoria_id " +
                "left join status_itens si on pi.fk_status_id = si.id " +
                "left join pagamento pg on pg.fk_usuario_conta_id = uc.id " +
                "left join meio_pagamento mpg on mpg.id_meio_pagamento = pg.fk_id_meio_pagamento " +

                "where c.id=" + id_conta;


                IEnumerable<ContaDetalhe> x = conexao.QueryAsync<ContaDetalhe>(sql,
                null,
                commandType: System.Data.CommandType.Text).Result;

                if (x.Count() > 0)
                {
                    string js = "[{";

                    js += "\"num_conta\":" + x.First().NUM_CONTA + ",";
                    js += "\"valor_total_conta\":" +  x.First().VALOR_TOTAL_CONTA + ",";
                    js += "\"desc_mesa\":" + "\"" + x.First().DESC_MESA + "\",";

                    var listaUsuarios = from a in x
                                     group a by a.USUARIO_ID into newGroup
                                     orderby newGroup.Key
                                     select newGroup;
                    double totalPago=0;
                    foreach (var item in listaUsuarios)
                    {
                        totalPago += item.First().VALOR_PAGO_CONTA_USUARIO;
                    }

                    js += "\"valor_total_pago\":" + totalPago + ",";

                    int loopUsuarios = 0;
                    js += "\"usuarios\":[";
                    foreach (var item in listaUsuarios)
                    {
                        if (loopUsuarios > 0)
                            js += ",";

                        js += "{";
                        js += "\"usuario_id\":" + item.First().USUARIO_ID + ",";
                        js += "\"id_usuario_conta\":" + item.First().ID_USUARIO_CONTA + ",";
                        js += "\"nome_usuario\":" + "\"" + item.First().NOME_USUARIO + "\",";
                        js += "\"cpf_usuario\":" + "\"" + item.First().CPF_USUARIO + "\",";
                        js += "\"valor_conta_usuario\":" + item.First().VALOR_CONTA_USUARIO + ",";
                        js += "\"valor_pago_conta_usuario\":" + item.First().VALOR_PAGO_CONTA_USUARIO + ",";
                        js += "\"usuario_meio_pagamento\":" + "\"" + item.First().USUARIO_MEIO_PAGAMENTO + "\",";
                        js += "\"status_conta_usuario\":" + "\"" + item.First().STATUS_CONTA_USUARIO + "\",";

                        js += "\"itens\":[";
                        int loopItens = 0;
                        foreach (ContaDetalhe i in item)
                        {
                            if (loopItens > 0)
                                js += ",";

                            js += "{";
                            js += "\"categoria\":" + "\"" + i.ITEM_CATEGORIA + "\",";
                            js += "\"titulo\":" + "\"" + i.ITEM_TITULO + "\",";
                            js += "\"descricao\":" + "\"" + i.ITEM_DESCRICAO + "\",";
                            js += "\"item_qtd\":" + i.ITEM_QTD + ",";
                            js += "\"item_VALOR\":" + i.ITEM_VALOR + ",";
                            js += "\"item_VALOR_DESCONTO\":" + (i.ITEM_DESCONTO > 0 ? (i.ITEM_VALOR - (i.ITEM_VALOR * i.ITEM_DESCONTO)) : i.ITEM_VALOR) + ",";
                            js += "\"item_imagem\":" + "\"" + i.ITEM_IMAGEM + "\",";
                            js += "\"item_status\":" + "\"" + i.ITEM_STATUS + "\",";
                            js += "\"item_DESCONTO\":" + i.ITEM_DESCONTO;
                            js += "}";
                            loopItens += 1;
                            
                        }
                        loopUsuarios += 1;
                        js += "]";
                        js += "}";

                    }
                    js += "]";
                    js += "}]";
                    return js;
                }
                return "";
            }


        }


        /// <summary>
        /// Reecupera os pedidos do estabelecimento
        /// </summary>
        /// <param name="id_estabelecimento"></param>
        /// /// <param name="is_cozinha"></param>
        /// <returns></returns>
        public object GetDetalheContasNovo(int id_estabelecimento, int? idFuncionario, int? is_cozinha, int? status_conta, int? statusItem)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "select " +
                "c.id as num_conta, " +
                "uc.id as id_usuario_conta," +
                "u.id as usuario_id," +
                "u.nome as nome_usuario," +
                "u.cpf as cpf_usuario," +
                "m.descricao as desc_mesa," +
                "c.total as valor_total_conta," +
                "uc.total as valor_conta_usuario," +
                "cat.descricao as item_categoria," +
                "i.titulo as item_titulo," +
                "i.descricao as item_descricao," +
                "pi.qtd as item_qtd, " +
                "i.valor as item_valor, " +
                "i.imagem as item_imagem, " +
                "i.is_cozinha as item_is_cozinha, " +
                "si.descricao as item_status, " +
                "pi.id as item_pedidoitem_id, " +
                "uf.nome as nome_funcionario, " +
                "sc.descricao as status_conta, " +
                "sc2.descricao as status_conta_usuario, " +
                "pi.desconto as item_desconto, " +
                "p.id as pedido_id, " +
                "p.datahora as pedido_datahora, " +
                "date_add(p.datahora, interval i.tempo_estimado_min minute) as item_prev_min, "+
                "date_add(p.datahora, interval i.tempo_estimado_max minute) as item_prev_max,  "+
                "pg.valor valor_pago_conta_usuario, " +
                "mpg.descricao usuario_meio_pagamento " +

                "from conta c " +
                "left join status_conta sc on sc.id = c.id_status " +
                "left join usuario_conta uc on c.id = uc.id_conta " +
                "left join status_conta sc2 on sc2.id = uc.status_conta_usuario " +
                "left join usuario u on uc.id_usuario = u.id " +
                "left join mesa m on c.id_mesa = m.id " +
                "left join pedido p on  uc.id = p.fk_usuario_conta_id " +
                "left join pedido_itens pi on  p.id = pi.id_pedido " +
                "left join itens i on i.id = pi.fk_itens_id " +
                "left join categoria cat on cat.id = i.fk_categoria_id " +
                "left join status_itens si on pi.fk_status_id = si.id " +
                "left join funcionarios_mesa fm on fm.fk_id_mesa = m.id " +
                "left join funcionario f on f.id = fm.fk_id_funcionario and f.id_estabelecimento = m.fk_id_estabelecimento " +
                "left join usuario uf on f.id_usuario = uf.id " +
                "left join pagamento pg on pg.fk_usuario_conta_id = uc.id " +
                "left join meio_pagamento mpg on mpg.id_meio_pagamento = pg.fk_id_meio_pagamento " +
                " where m.fk_id_estabelecimento=" + id_estabelecimento;

                sql += (idFuncionario == null ? "" : " and fm.fk_id_funcionario =" + idFuncionario);
                sql += (status_conta  == null ? "" : " and c.id_status =" + status_conta);
                sql += (is_cozinha    == null ? "" : " and i.is_cozinha =" + is_cozinha);
                sql += (statusItem == null ? "" : " and pi.fk_status_id =" + statusItem);


                IEnumerable<ContaDetalhe> x = conexao.Query<ContaDetalhe>(sql,
                null,
                commandType: System.Data.CommandType.Text);

                if (x != null && x.Count() > 0)
                {
                    //Agupa as contas ditintamente
                    var listaContas = from a in x
                                      group a by a.NUM_CONTA into newGroup
                                      orderby newGroup.Key
                                      select newGroup;


                    string js = "{\"contas\":[";
                    
                    int loopConta = 0;
                    foreach (var conta in listaContas)
                    {
                        if (loopConta > 0)
                            js += ",";

                        js += "{";


                        js += "\"num_conta\":" + conta.First().NUM_CONTA + ",";
                        js += "\"valor_total_conta\":" + conta.First().VALOR_TOTAL_CONTA + ",";
                        js += "\"status_conta\":" + "\"" + conta.First().STATUS_CONTA + "\",";
                        js += "\"desc_mesa\":" + "\"" + conta.First().DESC_MESA + "\",";
                        js += "\"garcom\":" + "\"" + conta.First().NOME_FUNCIONARIO + "\",";

                        var listaUsuarios = from a in conta
                                            group a by a.USUARIO_ID into newGroup
                                            orderby newGroup.Key
                                            select newGroup;

                        double totalPago = 0;
                        foreach (var item in listaUsuarios)
                        {
                            totalPago += item.First().VALOR_PAGO_CONTA_USUARIO;
                        }

                        js += "\"valor_total_pago\":" + totalPago + ",";

                        int loopUsuarios = 0;
                        js += "\"usuarios\":[";
                        foreach (var item in listaUsuarios)
                        {
                            if (loopUsuarios > 0)
                                js += ",";

                            js += "{";
                            js += "\"usuario_id\":" + item.First().USUARIO_ID + ",";
                            js += "\"id_usuario_conta\":" + item.First().ID_USUARIO_CONTA + ",";
                            js += "\"nome_usuario\":" + "\"" + item.First().NOME_USUARIO + "\",";
                            js += "\"cpf_usuario\":" + "\"" + item.First().CPF_USUARIO + "\",";
                            js += "\"valor_conta_usuario\":" + item.First().VALOR_CONTA_USUARIO + ",";
                            js += "\"valor_pago_conta_usuario\":" + item.First().VALOR_PAGO_CONTA_USUARIO + ",";
                            js += "\"usuario_meio_pagamento\":" + "\"" + item.First().USUARIO_MEIO_PAGAMENTO + "\",";
                            js += "\"status_conta_usuario\":" + "\"" + item.First().STATUS_CONTA_USUARIO + "\",";

                            var listaPedidos = from a in item
                                                group a by a.PEDIDO_ID into newGroup
                                                orderby newGroup.Key
                                                select newGroup;
                            js += "\"pedidos\":[";
                            int loopPedidos = 0;
                            foreach (var ped in listaPedidos)
                            {
                                if(loopPedidos>0)
                                    js += ",";

                                js += "{";

                                js += "\"pedido_id\":" + ped.First().PEDIDO_ID + ",";
                                js += "\"pedido_datahora\":" + "\"" + ped.First().PEDIDO_DATAHORA + "\",";

                                js += "\"itens\":[";
                                int loopItens = 0;
                                foreach (ContaDetalhe i in ped)
                                {

                                    if (i.ITEM_DESCRICAO != null && (is_cozinha == null || i.ITEM_IS_COZINHA == is_cozinha))
                                    {
                                        if (loopItens > 0)
                                            js += ",";

                                        js += "{";

                                        js += "\"categoria\":" + "\"" + i.ITEM_CATEGORIA + "\",";
                                        js += "\"titulo\":" + "\"" + i.ITEM_TITULO + "\",";
                                        js += "\"descricao\":" + "\"" + i.ITEM_DESCRICAO + "\",";
                                        js += "\"item_qtd\":" + i.ITEM_QTD + ",";
                                        js += "\"item_VALOR\":" + i.ITEM_VALOR + ",";
                                        js += "\"item_imagem\":" + "\"" + i.ITEM_IMAGEM + "\",";
                                        js += "\"item_status\":" + "\"" + i.ITEM_STATUS + "\",";
                                        js += "\"item_id\":" + i.ITEM_PEDIDOITEM_ID + ",";
                                        js += "\"item_desconto\":" + i.ITEM_DESCONTO + ",";
                                        js += "\"item_VALOR_DESCONTO\":" + (i.ITEM_DESCONTO >0?(i.ITEM_VALOR -(i.ITEM_VALOR*i.ITEM_DESCONTO)):i.ITEM_VALOR) + ",";
                                        js += "\"item_is_cozinha\":" + i.ITEM_IS_COZINHA + ",";
                                        js += "\"item_prev_min\":" + "\"" + i.ITEM_PREV_MIN + "\",";
                                        js += "\"item_prev_max\":" + "\"" + i.ITEM_PREV_MAX + "\"";
                                        js += "}";
                                        loopItens += 1;
                                    }
                                }
                                loopUsuarios += 1;
                                //Fecha o array de itens
                                js += "]";
                                js += "}";
                                loopPedidos += 1;
                            }
                            //Fecha o array de pedidos
                            js += "]";
                            js += "}";
                        }
                        js += "]"; //Fecha a lista de usuários
                        js += "}"; //Fecha a lista de contas
                        loopConta += 1;
                    }
                    js += "]}";

                    object o = JsonConvert.DeserializeObject(js);
                    string json2 = JsonConvert.SerializeObject(o, Formatting.Indented);

                    return json2;
                } // FIM DO IF DE CONTA
            }
            //Retorno se está vazio
            return "[{}]";
        }
    }
}
