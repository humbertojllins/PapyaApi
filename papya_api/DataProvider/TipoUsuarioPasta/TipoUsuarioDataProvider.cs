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
    public class TipoUsuarioDataProvider:ITipoUsuarioDataProvider
    {

        private readonly IConfiguration _configuration;

        public TipoUsuarioDataProvider()
        {

        }

        public TipoUsuarioDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public  Task AddTipoUsuario(TipoUsuario tipoUsuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "insert into tipo_usuario (descricao, abrev, status)" +
                    " values(" + 
                    "'" + tipoUsuario.Descricao + "'," +
                    "'" + tipoUsuario.Abrev + "'," +
                    "'" + tipoUsuario.Status + "')";
                return conexao.ExecuteAsync(sql
                    , commandType: System.Data.CommandType.Text);
            }
        }

        public  Task DeleteTipoUsuario(int CodTipoUsuario)
        {
            throw new NotImplementedException();
        }

        public async Task<TipoUsuario> GetTipoUsuario(int CodTipoUsuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<TipoUsuario>(
                    "select * from tipo_usuario where id=" + CodTipoUsuario.ToString(),
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<IEnumerable<TipoUsuario>> GetTipoUsuarios()
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QueryAsync<TipoUsuario>(
                "select * from tipo_usuario",
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public async Task UpdateTipoUsuario(TipoUsuario tipoUsuario)
        {
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
        }
    }
}
