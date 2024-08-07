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
    public class TipoEstabelecimentoDataProvider:ITipoEstabelecimentoDataProvider
    {

        private readonly IConfiguration _configuration;

        public TipoEstabelecimentoDataProvider()
        {

        }

        public TipoEstabelecimentoDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public  Task AddTipoEstabelecimento(TipoEstabelecimento estabelecimento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "insert into tipo_estabelecimento (descricao, imagem)" +
                    " values(" + 
                    "'" + estabelecimento.Descricao + "'," +
                    "'" + estabelecimento.Imagem + "')";
                return conexao.ExecuteAsync(sql
                    , commandType: System.Data.CommandType.Text);
            }
        }

        public  Task DeleteTipoEstabelecimento(int CodTipoEstabelecimento)
        {
            throw new NotImplementedException();
        }

        public async Task<TipoEstabelecimento> GetTipoEstabelecimento(int id)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<TipoEstabelecimento>(
                    "select * from tipo_estabelecimento where id=" + id,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }



        //public object GetTipoEstabelecimentos(int? qtdLista)
        //{
        //    using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
        //    {
        //        string sql = "select ";
        //        sql += qtdLista == null ? " " : " top " + qtdLista + "";
        //        sql += " * from TIPO_ESTABELECIMENTO ";
        //        conexao.Open();

        //        IEnumerable<TipoEstabelecimento> x = conexao.Query<TipoEstabelecimento>(sql, null,
        //       commandType: System.Data.CommandType.Text);


        //        string js = "{";

        //        int loopPromocoes = 0;
        //        js += "\"tipoestabelecimento\":[";
        //        foreach (var item in x)
        //        {
        //            if (loopPromocoes > 0)
        //                js += ",";

        //            js += "{";
        //            js += "\"id\":" + item.Id + ",";
        //            js += "\"descricao\":" + "\"" + item.Descricao + "\",";
        //            js += "\"imagem\":" + "\"" + item.Imagem + "\"";
        //            js += "}";

        //            loopPromocoes += 1;

        //        }
        //        js += "]}";
        //        return js;
        //    }
        //}
        public object GetTipoEstabelecimentos(int? qtdLista)
        {
            using (var conexao = new MySqlConnection(_configuration.GetConnectionString("tingledb")))
            {
                string sql = "";
                sql = "select * from tipo_estabelecimento ";
                sql += qtdLista == null ? ";" : " limit " + qtdLista + ";";


                IEnumerable<TipoEstabelecimento> x = conexao.QueryAsync<TipoEstabelecimento>(sql, null,
               commandType: System.Data.CommandType.Text).Result;


                string js = "{";

                int loopPromocoes = 0;
                js += "\"tipoestabelecimento\":[";
                foreach (var item in x)
                {
                    if (loopPromocoes > 0)
                        js += ",";

                    js += "{";
                    js += "\"id\":" + item.Id + ",";
                    js += "\"descricao\":" + "\"" + item.Descricao + "\",";
                    js += "\"imagem\":" + "\"" + item.Imagem + "\"";
                    js += "}";

                    loopPromocoes += 1;

                }
                js += "]}";
                return js;
            }
        }

        public async Task UpdateTipoEstabelecimento(TipoEstabelecimento estabelecimento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "update tipo_estabelecimento set descricao='" + estabelecimento.Descricao + "'," +
                    "imagem='" + estabelecimento.Imagem + "'" +
                   " where id=" + estabelecimento.Id;
                await conexao.ExecuteAsync(sql
                    ,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }
    }
}
