using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;

namespace papya_api.DataProvider
{
    public class ContaDataProvider : IContaDataProvider
    {

        private readonly IConfiguration _configuration;

        public ContaDataProvider()
        {

        }
        public ContaDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object AddConta(int? idMesa, int? idUsuario, int? publico)
        {
            object validacao = ValidaAberturaConta(idMesa, idUsuario).ToString();


            if (validacao.Equals("0"))
            {
                using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
                {
                    conexao.Open();
                    try
                    {
                        var sql = "insert into conta (id_mesa, id_status,abertura_data)" +
                            " values(" +
                            "" + idMesa + "," +
                            "1," +
                            "sysdate());" +
                            "insert into usuario_conta(id_usuario, id_conta, publico, abriu_conta)" +
                            " values(" +
                            "" + idUsuario + "," +
                            "LAST_INSERT_ID()," +
                            "" + publico + ",1);";
                        conexao.ExecuteScalar(sql
                            , commandType: System.Data.CommandType.Text);
                        return GetContas(idUsuario).Result;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                return new
                {
                    retorno = "Retorno:406"
                };
            }

        }

        public object AddContaUsuario(int? idConta, int? idUsuario, int? publico)
        {
            object validacao = ValidaUsuarioEntrarConta(idUsuario).ToString();

            if (validacao.Equals("0"))
            {
                using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
                {
                    conexao.Open();
                    var sql = "insert into usuario_conta(id_usuario, id_conta, publico, status_conta_usuario)" +
                        " values(" +
                        "" + idUsuario + "," +
                        "" + idConta + "," +
                        "" + publico + "," +
                        "1);";
                    conexao.Execute(sql
                        , commandType: System.Data.CommandType.Text);
                    return GetContas(idUsuario).Result;
                }
            }
            else
            {
                return new
                {
                    retorno = "Retorno:406"
                };
            }

        }

        public object DeleteConta(int idConta)
        {
            throw new NotImplementedException();
        }

        public async Task<Conta> GetConta(int? idConta)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Conta>(
                    "select * from conta where id=" + idConta.ToString(),
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }


        public async Task<IEnumerable<object>> GetContas(int? id_Usuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "select c.id as num_conta, " +
                "c.total as valor_conta," +
                "c.abertura_data as data_abertura_conta," +
                "c.fechamento_data as data_fechamento_conta," +
                "sc.descricao as status_conta," +
                "sc2.descricao as status_conta_usuario, " +
                "u.nome as nome_usuario," +
                "u.cpf as cpf_usuario," +
                "u.nascimento as nascimento_usuario," +
                "uc.id as id_usuario_conta," +
                "uc.publico as perfil_publico," +
                "m.id as id_mesa," +
                "m.descricao as desc_mesa," +
                "e.id_estabelecimento as codigo_estabelecimento," +
                "e.nome as nome_estabelecimento," +
                "e.cnpj as cnpj_estabelecimento, " +
                "e.imagem as imagem_estabelecimento " +
                "from conta c " +
                "left join status_conta sc on sc.id = c.id_status " +
                "left join usuario_conta uc on c.id = uc.id_conta " +
                "left join status_conta sc2 on sc2.id = uc.status_conta_usuario " +
                "left join usuario u on uc.id_usuario = u.id " +
                "left join mesa m on c.id_mesa = m.id " +
                "left join estabelecimento e on e.id_estabelecimento = m.fk_id_estabelecimento " +
                "left join tipo_estabelecimento te on te.id = e.fk_tipo_estabelecimento_id " +
                "where u.id=" + id_Usuario +
                " order by c.id desc;";


                return await conexao.QueryAsync<object>(sql, 
                null,
                commandType: System.Data.CommandType.Text);
            }
        }
        public object ValidaAberturaConta(int? idMesa, int? idUsuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "select count(1)retorno " +
                "from conta c " +
                "left join usuario_conta uc on c.id = uc.id_conta " +
                "where c.id_status=1 " +
                "and (c.id_mesa=" + idMesa + " or uc.id_usuario=" + idUsuario + ");";

                return conexao.QuerySingleOrDefault<string>(sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public object ValidaUsuarioEntrarConta(int? idUsuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "select count(1)retorno " +
                "from conta c " +
                "left join usuario_conta uc on c.id = uc.id_conta " +
                "where c.id_status=1 " +
                "and uc.id_usuario=" + idUsuario + ";";

                return conexao.QuerySingleOrDefault<string>(sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }



        /*
public async Task<IEnumerable<string>> Getcontas()
{
   using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
   {
       conexao.Open();
       return await conexao.QueryAsync<string>(
       "select * from CONTA",
       null,
       commandType: System.Data.CommandType.Text);
   }
}
*/

        public object UpdateConta(Conta conta)
        {
            throw new NotImplementedException();
            /*
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "update tipo_usuario set descricao='" + tipoUsuario.Descricao + "'," +
                    "abrev='" + tipoUsuario.Abrev + "'," +
                    "Status='" + tipoUsuario.Status + "'" +
                    " where id=" + tipoUsuario.Id;
                await conexao.ExecuteAsync(sql
                    ,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
            */
        }

        public object Pagarconta(int idUsuarioConta)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "update usuario_conta set status_conta_usuario = " + Convert.ToInt32(StatusConta.SolicitaPagamento) +
                    " where id=" + idUsuarioConta;
                return conexao.Execute(sql
                    ,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public object Fecharconta(int idConta, float total, int meioPagamento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql1 = "(select id from usuario_conta where abriu_conta =1 and id_conta =" + idConta + ")";

                string sql = "insert into PAGAMENTO(valor,fk_id_meio_pagamento,fk_usuario_conta_id) values(" + total + "," + meioPagamento + "," + sql1 + ");";

                //Atualiza o status da conta para Fechada e adiciona a data do fechamento
                sql += "update conta set id_status=" + Convert.ToInt32(StatusConta.Fechada) + ",";
                sql += " fechamento_data='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'";
                sql += " where id=" + idConta + ";";

                //Atualiza o status da conta do usuário
                sql += "UPDATE usuario_conta SET status_conta_usuario =" + Convert.ToInt32(StatusConta.Fechada);
                sql += " where id_conta=" + idConta + ";";


                return conexao.Execute(sql, null, commandType: System.Data.CommandType.Text);
            }
        }

        public object FecharcontaParcial(int idConta, int idUsuarioConta, float total, int meioPagamento, bool ultimoClienteMesa)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "update usuario_conta set status_conta_usuario =" + Convert.ToInt32(StatusConta.Fechada);
                sql += " where id=" + idUsuarioConta + ";";

                sql += "insert into pagamento(valor,fk_id_meio_pagamento,fk_usuario_conta_id) values(" + total + "," + meioPagamento + "," + idUsuarioConta + ");";

                //Se for o último cliente da mesa, fecha a conta total
                if (ultimoClienteMesa == true)
                {
                    sql += "update usuario_conta set status_conta_usuario =" + Convert.ToInt32(StatusConta.Fechada);
                    sql += " where id_conta=" + idConta + ";";
                }
                return conexao.Execute(sql
                    ,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public bool ValidaFechamentoConta(out string mensagem, int idConta, float total, bool fechamentoTotal)
        {
            object totalPagar, usuariosNaMesa;

            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "select c.total - " +
                            "(select isnull(sum(p.valor),0)" +
                            " from usuario_conta uc" +
                            " left join pagamento p on p.fk_usuario_conta_id = uc.id and uc.status_conta_usuario = 2" +
                            " where  uc.id_conta = c.id" +
                            ") total_pagar" +
                            " from conta c" +
                            " where c.id =" + idConta;

                totalPagar = conexao.ExecuteScalar(sql, null, commandType: System.Data.CommandType.Text);


                sql = "select count(1) " +
                      " from usuario_conta uc" +
                      " where uc.id_conta = " + idConta +
                      " and uc.status_conta_usuario <> 2";
                usuariosNaMesa = conexao.ExecuteScalar(sql, null, commandType: System.Data.CommandType.Text);

            }

            totalPagar = totalPagar == null ? 0 : totalPagar;
            if (fechamentoTotal == true)
            {
                if (Math.Round(float.Parse(totalPagar.ToString()), 2) != Math.Round(total, 2))
                {
                    mensagem = "Valor pago deve ser igual ao total ou saldo restante da conta";
                    return false;
                }
            }
            else
            {
                if (Math.Round(float.Parse(totalPagar.ToString()), 2) < Math.Round(total, 2))
                {
                    mensagem = "Valor pago maior que o total da conta";
                    return false;
                }
                if (Math.Round(float.Parse(totalPagar.ToString()), 2) > Math.Round(total, 2) && Convert.ToInt32(usuariosNaMesa) == 1)
                {
                    mensagem = "Valor pago menor que o débito da conta";
                    return false;
                }
                if (Convert.ToInt32(usuariosNaMesa) == 1)
                {
                    mensagem = "FecharContaTotal";
                    return true;
                    //retornar para fechamento total da conta
                }
            }
            mensagem = "";
            return true;
        }


        public enum StatusConta : int
        {
            Aberta = 1,
            Fechada = 2,
            SolicitaPagamento = 3
        }
    }
}
