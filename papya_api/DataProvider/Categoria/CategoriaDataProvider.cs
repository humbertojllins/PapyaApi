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
    public class CategoriaDataProvider : ICategoriaDataProvider
    {

        private readonly IConfiguration _configuration;

        public CategoriaDataProvider()
        {

        }

        public CategoriaDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object AddCategoria(Categoria categoria)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "insert into categoria (descricao, fk_id_estabelecimento)" +
                    " values(" +
                    "'" + categoria.descricao + "'," +
                    "" + categoria.fk_id_estabelecimento + ");";
                sql += " select last_insert_id();";
                return conexao.ExecuteScalar(sql
                    , commandType: System.Data.CommandType.Text);
            }
        }
        public async Task<Categoria> GetCategoria(int idCategoria)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select * from categoria";
                sql += " where id=" + idCategoria + ";";
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Categoria>(
                    sql,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<IEnumerable<Categoria>> GetCategorias(int idEstabelecimento, int? qtdLista)
        {
            using (var conexao = new MySqlConnection(_configuration.GetConnectionString("tingledb")))
            {
                string sql = "";
                sql = "select * from categoria where fk_id_estabelecimento=" + idEstabelecimento.ToString();
                sql += qtdLista == null ? ";" : " limit " + qtdLista + ";";
                conexao.Open();
                return await conexao.QueryAsync<Categoria>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public object UpdateCategoria(Categoria categoria)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "update categoria set descricao='" + categoria.descricao + "'" +
                   " where id=" + categoria.id;
                return conexao.Execute(sql
                    ,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }
    }
}
