using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using MySql.Data.MySqlClient;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;

namespace papya_api.DataProvider
{
    public class TipoFuncionarioDataProvider:ITipoFuncionarioDataProvider
    {

        private readonly IConfiguration _configuration;

        public TipoFuncionarioDataProvider()
        {

        }

        public TipoFuncionarioDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public  Task AddTipoFuncionario(TipoFuncionario tipoFuncionario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "insert into tipo_funcionario (descricao, abrev, status)" +
                    " values(" + 
                    "'" + tipoFuncionario.Descricao + "'," +
                    "'" + tipoFuncionario.Abrev + "'," +
                    "'" + tipoFuncionario.Status + "')";
                return conexao.ExecuteAsync(sql
                    , commandType: System.Data.CommandType.Text);
            }
        }

        public  Task DeleteTipoFuncionario(int CodTipoFuncionario)
        {
            throw new NotImplementedException();
        }

        public async Task<TipoFuncionario> GetTipoFuncionario(int CodTipoFuncionario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<TipoFuncionario>(
                    "select * from tipo_funcionario where id=" + CodTipoFuncionario.ToString(),
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<IEnumerable<TipoFuncionario>> GetTipoFuncionarios()
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QueryAsync<TipoFuncionario>(
                "select * from tipo_funcionario",
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public async Task UpdateTipoFuncionario(TipoFuncionario tipoFuncionario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "update tipo_funcionario set descricao='" + tipoFuncionario.Descricao + "'," +
                    "abrev='" + tipoFuncionario.Abrev + "'," +
                    "Status='" + tipoFuncionario.Status + "'" +
                    " where id=" + tipoFuncionario.Id;
                await conexao.ExecuteAsync(sql
                    ,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }
    }
}
