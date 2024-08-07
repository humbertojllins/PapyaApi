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
    public class MesaDataProvider : IMesaDataProvider
    {

        private readonly IConfiguration _configuration;

        public MesaDataProvider()
        {

        }

        public MesaDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object AddMesa(Mesa mesa)
        {
            int idMesa = 0;
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                try
                {
                    conexao.Open();
                    var sql = "insert into mesa (descricao, fk_id_estabelecimento)" +
                        " values(" +
                        "'" + mesa.Descricao + "'," +
                        "" + mesa.fk_id_estabelecimento + ");";
                    sql += " select last_insert_id();";
                    idMesa = Convert.ToInt32(conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    var sql = "insert into funcionarios_mesa (fk_id_funcionario, fk_id_mesa)" +
                    " values(" +
                    mesa.fk_Id_funcionario + "," +
                    idMesa + ")";

                    ret = conexao.Execute(sql, commandType: System.Data.CommandType.Text);
                }
                return ret;
                
            }
        }

        public object DeleteMesa(int CodMesa)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update mesa set status=0" +
                    " where id=" + CodMesa.ToString();

                ret= conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);
                conexao.Close();
            }
            return ret;
        }

        public async Task<Mesa> GetMesa(int codMesa)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Mesa>(
                    "select * from mesa where status=1 and id=" + codMesa.ToString(),
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<IEnumerable<Mesa>> GetMesas(int idEstabelecimento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select M.id, M.descricao, M.fk_id_estabelecimento,F.id as fk_Id_funcionario,FM.id as id_funcionario_mesa , U.nome AS nome_funcionario ";
                sql+= "from mesa m ";
                sql += "left join funcionarios_mesa fm on m.id = fm.fk_id_mesa ";
                sql += "left join funcionario f on f.id = fm.fk_id_funcionario ";
                sql += "left join usuario u on u.id = f.id_usuario ";
                sql += " where status=1 and fk_id_estabelecimento=" + idEstabelecimento;
                conexao.Open();
                return await conexao.QueryAsync<Mesa>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public object UpdateMesa(Mesa Mesa)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update mesa SET " +
                    "descricao='" + Mesa.Descricao + "'," +
                    "fk_id_estabelecimento=" + Mesa.fk_id_estabelecimento +
                    " where id=" + Mesa.Id.ToString();

                ret=  conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);
                conexao.Close();
            }            
            return ret;
        }
    }
}
