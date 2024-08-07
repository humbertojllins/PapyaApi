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
    public class StatusItemDataProvider : IStatusItemDataProvider
    {

        private readonly IConfiguration _configuration;

        public StatusItemDataProvider()
        {

        }

        public StatusItemDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task AddStatusItem(StatusItem statusItem)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "insert into status_itens (descricao)" +
                    " values(" +
                    "'" + statusItem.Descricao + "')";
                return conexao.ExecuteAsync(sql
                    , commandType: System.Data.CommandType.Text);
            }
        }

        public Task DeleteStatusItem(int CodStatusItem)
        {
            throw new NotImplementedException();
        }

        public async Task<StatusItem> GetStatusItem(int codStatusItem)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<StatusItem>(
                    "select * from status_itens where id=" + codStatusItem.ToString(),
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<IEnumerable<StatusItem>> GetStatusItems()
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QueryAsync<StatusItem>(
                "select * from status_itens",
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public Task UpdateStatusItem(StatusItem StatusItem)
        {
            throw new NotImplementedException();
        }
    }
}
