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
    public class FuncionarioMesaDataProvider: IFuncionarioMesaDataProvider
    {

        private readonly IConfiguration _configuration;

        public FuncionarioMesaDataProvider()
        {

        }

        public FuncionarioMesaDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public  object AddFuncionarioMesa(FuncionarioMesa FuncionarioMesa)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "insert into funcionarios_mesa (fk_id_funcionario, fk_id_mesa)" +
                    " values(" +
                    FuncionarioMesa.Fk_Id_funcionario + "," +
                    FuncionarioMesa.Fk_Id_mesa + ")";
                ret = conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);
                conexao.Close();
            }
            return ret;
        }

        public int DeleteFuncionarioMesa(int CodFuncionarioMesa)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "delete from funcionarios_mesa " +
                    " where id=" + CodFuncionarioMesa.ToString();

                var ret = conexao.ExecuteAsync(sql
                    , commandType: System.Data.CommandType.Text);
                return ret.Result;
            }
        }

        public async Task<IEnumerable<FuncionarioMesa>> GetFuncionarioMesas(int idMesa)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "";
                sql = "select * from funcionarios_mesa where fk_id_mesa=" + idMesa.ToString();
                conexao.Open();
                return await conexao.QueryAsync<FuncionarioMesa>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }
    }
}
