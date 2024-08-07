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

        public Task AddCategoria(Categoria categoria)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "insert into categoria (descricao, fk_id_estabelecimento, imagem)" +
                    " values(" +
                    "'" + categoria.Descricao + "'," +
                    "" + categoria.Fk_Id_estabelecimento + "," +
                    "'" + categoria.Imagem + "')";
                return conexao.ExecuteAsync(sql
                    , commandType: System.Data.CommandType.Text);
            }
        }

        public Task DeleteCategoria(int CodCategoria)
        {
            throw new NotImplementedException();
        }

        public async Task<Categoria> GetCategoria()
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Categoria>(
                    "select * from categoria",
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        //public async Task<IEnumerable<Categoria>> GetCategorias(int idEstabelecimento, int? qtdLista)
        //{
        //    using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
        //    {
        //        string sql = "select ";
        //        sql += qtdLista == null ? " " : " top " + qtdLista + " ";
        //        sql += " * from CATEGORIA where fk_id_estabelecimento=" + idEstabelecimento.ToString();
        //        sql += qtdLista == null ? ";" : " limit " + qtdLista + ";";
        //        conexao.Open();
        //        return await conexao.QueryAsync<Categoria>(
        //        sql,
        //        null,
        //        commandType: System.Data.CommandType.Text);
        //    }
        //}

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

        public async Task UpdateCategoria(Categoria categoria)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "update categoria set descricao='" + categoria.Descricao + "'," +
                    "imagem='" + categoria.Imagem + "'" +
                   " where id=" + categoria.Id;
                await conexao.ExecuteAsync(sql
                    ,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }
    }
}
