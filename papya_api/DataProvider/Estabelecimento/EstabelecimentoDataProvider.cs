using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using papya_api.Services;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;

namespace papya_api.DataProvider
{
    public class EstabelecimentoDataProvider:IEstabelecimentoDataProvider
    {

        private readonly IConfiguration _configuration;
        Global util = new Global();

        public EstabelecimentoDataProvider()
        {

        }

        public EstabelecimentoDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public  object AddEstabelecimento(Estabelecimento estabelecimento)
        {
            object idEstabelecimento=0;
            string senhaCript = util.CalculateMD5Hash(estabelecimento.Senha);
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                try
                {
                    conexao.Open();
                    var sql = "insert into estabelecimento (nome,cnpj,endereco,numero,longitude,latitude,fk_tipo_estabelecimento_id,imagem,senha)" +
                        " values(" +
                        "'" + estabelecimento.Nome + "'," +
                        "'" + estabelecimento.Cnpj + "'," +
                        "'" + estabelecimento.Endereco + "'," +
                        "'" + estabelecimento.Numero + "'," +
                        "'" + estabelecimento.Longitude + "'," +
                        "'" + estabelecimento.Latitude + "'," +
                        "" + estabelecimento.fk_Tipo_Estabelecimento_id + "," +
                        "'" + estabelecimento.Imagem + "'," +
                        "'" + senhaCript + "');";
                    sql += "select LAST_INSERT_ID();";

                    idEstabelecimento = conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    var sql = "INSERT INTO categoria(descricao,fk_id_estabelecimento)VALUES('Refeição'," + idEstabelecimento + ");";
                    sql += "INSERT INTO categoria(descricao,fk_id_estabelecimento)VALUES('Petisco'," + idEstabelecimento + ");";
                    sql += "INSERT INTO categoria(descricao,fk_id_estabelecimento)VALUES('Cerveja'," + idEstabelecimento + ");";
                    sql += "INSERT INTO categoria(descricao,fk_id_estabelecimento)VALUES('Drinks'," + idEstabelecimento + ");";
                    sql += "INSERT INTO categoria(descricao,fk_id_estabelecimento)VALUES('Suco'," + idEstabelecimento + ");";
                    sql += "INSERT INTO categoria(descricao,fk_id_estabelecimento)VALUES('Lanche'," + idEstabelecimento + ");";
                    //var sql = "call sp_CadCategoria(" + idEstabelecimento + ");";
                    conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
                }

                return idEstabelecimento;

            }
        }

        public  Task DeleteEstabelecimento(int CodEstabelecimento)
        {
            throw new NotImplementedException();
        }

        public async Task<Estabelecimento> GetEstabelecimento(int id)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Estabelecimento>(
                    "select * from estabelecimento where id_estabelecimento=" + id,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        //public object GetEstabelecimentos(decimal? latitude, decimal? longitude, int? qtdLista, int? idTipoEstabelecimento)
        // {
        //     decimal? lat = latitude == null ? 0 : latitude;
        //     decimal? lon = longitude == null ? 0 : longitude;

        //     using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
        //     {
        //         string sql = "";
        //         conexao.Open();
        //         sql = "select ";
        //         sql += qtdLista == null ? "" : " top " + qtdLista + " ";
        //         sql += " id_estabelecimento,nome,observacao as descricao,cnpj,endereco,numero,longitude,latitude,imagem, " +
        //         "(" +
        //                 "acos(sin(radians(latitude))" +
        //                 "* sin(radians(" + lat + "))" +
        //                 "+ cos(radians(latitude))" +
        //                 "* cos(radians(" + lat + "))" +
        //                 "* cos(radians(longitude) " +
        //                 "- radians(" + lon + " ))) * 6378 " +
        //                 ") distancia_km " +
        //         " from ESTABELECIMENTO ";

        //         sql += (idTipoEstabelecimento == null ? "" : " where fk_tipo_estabelecimento_id =" + idTipoEstabelecimento);
        //         sql += " ORDER BY 9 ASC";



        //         IEnumerable<Estabelecimento> x = conexao.Query<Estabelecimento>(
        //         sql,
        //         null,
        //         commandType: System.Data.CommandType.Text);

        //         string js = "{";

        //         int loopPromocoes = 0;
        //         js += "\"estabelecimento\":[";
        //         foreach (var item in x)
        //         {
        //             if (loopPromocoes > 0)
        //                 js += ",";

        //             js += "{";
        //             js += "\"id_estabelecimento\":" + item.Id_Estabelecimento + ",";
        //             js += "\"nome\":" + "\"" + item.Nome + "\",";
        //             js += "\"descricao\":" + "\"" + item.Descricao + "\",";
        //             js += "\"cnpj\":" + "\"" + item.Cnpj + "\",";
        //             js += "\"endereco\":" + "\"" + item.Endereco + "\",";
        //             js += "\"numero\":" + "\"" + item.Numero + "\",";
        //             js += "\"longitude\":" + item.Longitude + ",";
        //             js += "\"latitude\":" + item.Latitude + ",";
        //             js += "\"imagem\":" + "\"" + item.Imagem + "\",";
        //             js += "\"distancia_km\":" + item.Distancia_km;

        //             js += "}";

        //             loopPromocoes += 1;

        //         }
        //         js += "]}";
        //         return js;

        //     }
        // }

        public object GetEstabelecimentos(float? latitude, float? longitude, int? qtdLista, int? idTipoEstabelecimento)
        {
            float? lat = latitude == null ? 0 : latitude;
            float? lon = longitude == null ? 0 : longitude;

            using (var conexao = new MySqlConnection(_configuration.GetConnectionString("tingledb")))
            {
                string sql = "";
                conexao.Open();
                sql = "select id_estabelecimento,nome,observacao as descricao,cnpj,endereco,numero,longitude,latitude,imagem, " +
                "(" +
                        "acos(sin(radians(latitude))" +
                        "* sin(radians(" + lat + "))" +
                        "+ cos(radians(latitude))" +
                        "* cos(radians(" + lat + "))" +
                        "* cos(radians(longitude) " +
                        "- radians(" + lon + " ))) * 6378 " +
                        ") distancia_km " +
                " from estabelecimento ";

                sql += (idTipoEstabelecimento == null ? "" : " where fk_tipo_estabelecimento_id =" + idTipoEstabelecimento);
                sql += " ORDER BY 9 ASC";
                sql += qtdLista == null ? ";" : " limit " + qtdLista + ";";


                IEnumerable<Estabelecimento> x = conexao.QueryAsync<Estabelecimento>(
                sql,
                null,
                commandType: System.Data.CommandType.Text).Result;

                string js = "{";

                int loopPromocoes = 0;
                js += "\"estabelecimento\":[";
                foreach (var item in x)
                {
                    if (loopPromocoes > 0)
                        js += ",";

                    js += "{";
                    js += "\"id_estabelecimento\":" + item.Id_Estabelecimento + ",";
                    js += "\"nome\":" + "\"" + item.Nome + "\",";
                    js += "\"descricao\":" + "\"" + item.Descricao + "\",";
                    js += "\"cnpj\":" + "\"" + item.Cnpj + "\",";
                    js += "\"endereco\":" + "\"" + item.Endereco + "\",";
                    js += "\"numero\":" + "\"" + item.Numero + "\",";
                    js += "\"longitude\":" + item.Longitude + ",";
                    js += "\"latitude\":" + item.Latitude + ",";
                    js += "\"imagem\":" + "\"" + item.Imagem + "\",";
                    js += "\"distancia_km\":" + item.Distancia_km;

                    js += "}";

                    loopPromocoes += 1;

                }
                js += "]}";
                return js;

            }
        }

        public Task<IEnumerable<Estabelecimento>> GetEstabelecimentos(int idEstabelecimento)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateEstabelecimento(Estabelecimento estabelecimento)
        {
            //TODO:REFAZER
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string sql = "update estabelecimento set nome='" + estabelecimento.Nome + "'," +
                    "endereco='" + estabelecimento.Endereco + "'," +
                    "numero='" + estabelecimento.Numero + "'," +
                    "longitude='" + estabelecimento.Longitude + "'," +
                    "latitude='" + estabelecimento.Latitude + "'," +
                    "fk_tipo_estabelecimento_id=" + estabelecimento.fk_Tipo_Estabelecimento_id + "," +
                    "imagem='" + estabelecimento.Imagem + "'" +
                   " where id_estabelecimento=" + estabelecimento.Id_Estabelecimento;
                await conexao.ExecuteAsync(sql
                    ,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<Estabelecimento> GetEstabelecimentoCnpj(string cnpj)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Estabelecimento>(
                    "select * from estabelecimento where cnpj='" + cnpj + "'",
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }
        
    }
}
