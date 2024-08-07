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
    public class MeioPagamentoDataProvider : IMeioPagamentoDataProvider
    {

        private readonly IConfiguration _configuration;

        public MeioPagamentoDataProvider()
        {

        }

        public MeioPagamentoDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task AddMeioPagamento(MeioPagamento meioPagamento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "insert into meio_pagamento (descricao)" +
                    " values(" +
                    "'" + meioPagamento.Descricao + "')";
                return conexao.ExecuteAsync(sql
                    , commandType: System.Data.CommandType.Text);
            }
        }

        public Task DeleteMeioPagamento(int idMeioPagamento)
        {
            throw new NotImplementedException();
        }

        public async Task<MeioPagamento> GetMeioPagamento(int idMeioPagamento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<MeioPagamento>(
                    "select * from meio_pagamento where id_meio_pagamento=" + idMeioPagamento.ToString(),
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<IEnumerable<MeioPagamento>> GetMeioPagamentos()
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QueryAsync<MeioPagamento>(
                "select * from meio_pagamento",
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public Task UpdateMeioPagamento(MeioPagamento MeioPagamento)
        {
            throw new NotImplementedException();
        }


    }
}
