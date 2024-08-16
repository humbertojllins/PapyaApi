using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Newtonsoft.Json.Linq;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;

namespace papya_api.DataProvider
{
    public class PromocoesDataProvider : IPromocoesDataProvider
    {

        private readonly IConfiguration _configuration;

        public PromocoesDataProvider()
        {

        }

        public PromocoesDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object AddPromocao(IEnumerable<Promocoes> listaPromocoes)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                string descricao = listaPromocoes.First().DESCRICAO;
                string data_vigencia = listaPromocoes.First().DATA_VIGENCIA;
                int validade = listaPromocoes.First().VALIDADE;
                int? fk_id_estabelecimento = listaPromocoes.First().ID_ESTABELECIMENTO;
                float desconto = listaPromocoes.First().DESCONTO;


                var sql = "insert into promocoes (descricao, data_vigencia,validade,fk_id_estabelecimento,desconto)" +
                    " values('" + descricao + "','" + data_vigencia + "','" + validade + "',"+ fk_id_estabelecimento + "," + desconto + ");";
                sql += " select last_insert_id();";

                int id_promocao = Convert.ToInt32(conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text));
                sql = "";
                foreach (Promocoes item in listaPromocoes)
                {
                    sql += " insert into itens_PROMOCAO (id_promocao,id_item, qtd_item)";
                    sql += " values(" + id_promocao + ", ";
                    sql += item.CODIGO_ITEM + ",";
                    sql += item.QTD_ITEM + ");";
                }

                //sql += " SELECT id_conta FROM USUARIO_CONTA WHERE id =" + id_usuario_conta;


                conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
                return id_promocao;
                //return GetPromocoes(0,0,null, fk_id_estabelecimento);
            }
        }

        public object UpdatePromocao(IEnumerable<Promocoes> listaPromocoes)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                
                conexao.Open();

                int idpromocao = listaPromocoes.First().ID_PROMOCAO;
                string descricao = listaPromocoes.First().DESCRICAO;
                string data_vigencia = listaPromocoes.First().DATA_VIGENCIA;
                int validade = listaPromocoes.First().VALIDADE;
                int? fk_id_estabelecimento = listaPromocoes.First().ID_ESTABELECIMENTO;
                float desconto = listaPromocoes.First().DESCONTO;


                var sql = "update promocoes set " +
                    "descricao='" + descricao + "'," +
                    "data_vigencia='" + data_vigencia + "'," +
                    "validade=" + validade + "," +
                    "desconto=" + desconto +
                    " where id=" + idpromocao.ToString();
                var ret = conexao.ExecuteAsync(sql, commandType: System.Data.CommandType.Text);
                if (ret.IsCompletedSuccessfully == true)
                {
                    sql = " delete from itens_promocao where id_promocao=" + idpromocao + ";";
                    var ret1 = conexao.ExecuteAsync(sql, commandType: System.Data.CommandType.Text);

                    if (ret1.IsCompletedSuccessfully == true)
                    {
                        sql = "";
                        foreach (Promocoes item in listaPromocoes)
                        {
                            sql += " insert into itens_promocao (id_promocao,id_item, qtd_item)";
                            sql += " values(" + idpromocao + ", ";
                            sql += item.CODIGO_ITEM + ",";
                            sql += item.QTD_ITEM + ");";
                        }
                        conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
                    }
                    else
                    {
                        return ret.IsFaulted;
                    }
                }
                else
                {
                    return ret.IsFaulted;
                }

                return ((Task<int>)ret).IsCompletedSuccessfully;
            }
        }

        public object DeletePromocao(int idPromocao)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update promocoes SET " +
                    "status=0 " +
                    " where id=" + idPromocao.ToString();
                return conexao.Execute(sql, commandType: System.Data.CommandType.Text);
            }
        }

        //public object GetPromocoes(decimal latitude, decimal longitude, int? qtdLista, int? idestabelecimento)
        //{
        //    using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
        //    {
        //        conexao.Open();
        //        var sql = " SELECT ";
        //            sql += qtdLista == null ? " " : " top " + qtdLista;
        //            sql += " E.*, " +
        //            "I.id as codigo_item, " +
        //            "I.titulo, " +
        //            "I.descricao as descricao_item, " +
        //            "I.valor, " +
        //            "I.tempo_estimado_min, " +
        //            "I.tempo_estimado_max, " +
        //            "I.imagem as imagem_item, " +
        //            "IP.qtd_item " +
        //            "FROM( " +

        //            "SELECT " +
        //            "id_estabelecimento, " +
        //             "nome," +
        //             "cnpj," +
        //             "endereco," +
        //             "numero," +
        //             "longitude," +
        //             "latitude," +
        //             "E.imagem," +
        //             "TP.descricao as tipo_estabelecimento," +
        //             "P.id id_promocao," +
        //             "P.descricao," +
        //             "P.data_vigencia," +
        //             "P.imagem AS imagem_promocao," +
        //             "P.validade," +
        //             "(" +
        //                "acos(sin(radians(latitude))" +
        //                "* sin(radians(" + latitude + "))" +
        //                "+ cos(radians(latitude))" +
        //                "* cos(radians(" + latitude + "))" +
        //                "* cos(radians(longitude) " +
        //                "- radians(" + longitude + " ))) * 6378 " +
        //                ") distancia_km, " +
        //                "P.desconto " +
        //                " FROM ESTABELECIMENTO E " +
        //            "INNER JOIN TIPO_ESTABELECIMENTO TP ON TP.id = E.fk_tipo_estabelecimento_id " +
        //            "LEFT JOIN PROMOCOES P ON E.id_estabelecimento = P.fk_id_estabelecimento";
        //        sql += " WHERE P.status=1 ";
        //        //sql += " ORDER BY id_promocao desc";

        //        sql += " )E " +
        //            "INNER JOIN itens_PROMOCAO IP on IP.id_promocao = E.id_promocao " +
        //            "INNER JOIN itens I ON IP.ID_ITEM = I.ID ";
        //        sql += idestabelecimento == null ? " " : " WHERE id_estabelecimento= " + idestabelecimento.ToString();
        //        sql += " ORDER BY id_promocao desc ";


        //        IEnumerable<Promocoes> x = conexao.Query<Promocoes>(sql,null,
        //        commandType: System.Data.CommandType.Text);


        //        string js = "{";

        //        //js += "\"num_conta\":" + x.First().NUM_CONTA + ",";
        //        //js += "\"valor_total_conta\":" + x.First().VALOR_TOTAL_CONTA + ",";
        //        //js += "\"desc_mesa\":" + "\"" + x.First().DESC_MESA + "\",";

        //        var listaPromocoes = from a in x
        //                            group a by a.ID_PROMOCAO into newGroup
        //                            //orderby newGroup.Key descending
        //                            select newGroup;
        //        int loopPromocoes = 0;
        //        js += "\"promocoes\":[";
        //        foreach (var item in listaPromocoes)
        //        {
        //            if (loopPromocoes > 0)
        //                js += ",";

        //            js += "{";
        //            js += "\"id_promocao\":" + item.First().ID_PROMOCAO + ",";
        //            js += "\"id_estabelecimento\":" + item.First().ID_ESTABELECIMENTO + ",";
        //            js += "\"nome\":" + "\"" + item.First().NOME + "\",";
        //            js += "\"cnpj\":" + "\"" + item.First().CNPJ + "\",";
        //            js += "\"endereco\":" + "\"" + item.First().ENDERECO + "\",";
        //            js += "\"numero\":" + "\"" + item.First().NUMERO + "\",";
        //            js += "\"longitude\":" + "\"" + item.First().LONGITUDE + "\",";
        //            js += "\"latitude\":" + "\"" + item.First().LATITUDE + "\",";
        //            js += "\"imagem\":" + "\"" + item.First().IMAGEM + "\",";
        //            js += "\"tipo_estabelecimento\":" + "\"" + item.First().TIPO_ESTABELECIMENTO + "\",";
        //            js += "\"descricao\":" + "\"" + item.First().DESCRICAO + "\",";
        //            js += "\"imagem_promocao\":" + "\"" + item.First().IMAGEM_PROMOCAO + "\",";
        //            js += "\"validade\":" + item.First().VALIDADE + ",";
        //            js += "\"distancia_km\":" + item.First().DISTANCIA_KM + ",";
        //            js += "\"desconto\":" + item.First().DESCONTO + ",";


        //            js += "\"itens\":[";
        //            int loopItens = 0;
        //            foreach (Promocoes i in item)
        //            {
        //                if (loopItens > 0)
        //                    js += ",";

        //                js += "{";
        //                js += "\"codigo_item\":" + i.CODIGO_ITEM + ",";
        //                js += "\"titulo\":" + "\"" + i.TITULO + "\",";
        //                js += "\"descricao\":" + "\"" + i.DESCRICAO_ITEM + "\",";
        //                js += "\"valor\":" + i.VALOR + ",";
        //                js += "\"tempo_estimado_min\":" + i.TEMPO_ESPERADO_MIN + ",";
        //                js += "\"tempo_estimado_max\":" + i.TEMPO_ESPERADO_MAX + ",";
        //                js += "\"imagem\":" + "\"" + i.IMAGEM_PROMOCAO + "\",";
        //                js += "\"qtd_item\":" + i.QTD_ITEM;
        //                js += "}";
        //                loopItens += 1;

        //            }
        //            loopPromocoes += 1;
        //            js += "]";
        //            js += "}";

        //        }
        //        js += "]";
        //        js += "}";
        //        return js;

        //    }
        //}

        public object GetPromocoes(float latitude, float longitude, int? qtdLista, int? idestabelecimento)
        {
            using (var conexao = new MySqlConnection(_configuration.GetConnectionString("tingledb")))
            {
                conexao.Open();

                var sql = "select e.*, " +
                    "i.id as codigo_item, " +
                    "i.titulo, " +
                    "i.descricao as descricao_item, " +
                    "i.valor, " +
                    "i.tempo_estimado_min, " +
                    "i.tempo_estimado_max, " +
                    "i.imagem as imagem_item, " +
                    "ip.qtd_item " +
                    "from( " +

                    "select " +
                    "id_estabelecimento, " +
                     "nome," +
                     "cnpj," +
                     "endereco," +
                     "numero," +
                     "longitude," +
                     "latitude," +
                     "e.imagem," +
                     "tp.descricao as tipo_estabelecimento," +
                     "p.id id_promocao," +
                     "p.descricao," +
                     "p.data_vigencia," +
                     "p.imagem as imagem_promocao," +
                     "p.validade," +
                     "(" +
                        "acos(sin(radians(latitude))" +
                        "* sin(radians(" + latitude + "))" +
                        "+ cos(radians(latitude))" +
                        "* cos(radians(" + latitude + "))" +
                        "* cos(radians(longitude) " +
                        "- radians(" + longitude + " ))) * 6378 " +
                        ") distancia_km, " +
                        "p.desconto " +
                        " from tingledb.estabelecimento e " +
                    "inner join tingledb.tipo_estabelecimento tp on tp.id = e.fk_tipo_estabelecimento_id " +
                    "left join tingledb.promocoes p on e.id_estabelecimento = p.fk_id_estabelecimento";
                sql += " where p.status=1 ";
                sql += " order by id_promocao desc";
                sql += qtdLista == null ? " " : " limit " + qtdLista;

                sql += " )e " +
                    "inner join tingledb.itens_promocao ip on ip.id_promocao = e.id_promocao " +
                    "inner join tingledb.itens i on ip.id_item = i.id ";
                sql += idestabelecimento == null ? " " : " where id_estabelecimento= " + idestabelecimento.ToString();
                sql += " order by id_promocao desc; ";


                IEnumerable<Promocoes> x = conexao.QueryAsync<Promocoes>(sql, null,
                commandType: System.Data.CommandType.Text).Result;


                string js = "{";

                //js += "\"num_conta\":" + x.First().NUM_CONTA + ",";
                //js += "\"valor_total_conta\":" + x.First().VALOR_TOTAL_CONTA + ",";
                //js += "\"desc_mesa\":" + "\"" + x.First().DESC_MESA + "\",";

                var listaPromocoes = from a in x
                                     group a by a.ID_PROMOCAO into newGroup
                                     //orderby newGroup.Key descending
                                     select newGroup;
                int loopPromocoes = 0;
                js += "\"promocoes\":[";
                foreach (var item in listaPromocoes)
                {
                    if (loopPromocoes > 0)
                        js += ",";

                    js += "{";
                    js += "\"id_promocao\":" + item.First().ID_PROMOCAO + ",";
                    js += "\"id_estabelecimento\":" + item.First().ID_ESTABELECIMENTO + ",";
                    js += "\"nome\":" + "\"" + item.First().NOME + "\",";
                    js += "\"cnpj\":" + "\"" + item.First().CNPJ + "\",";
                    js += "\"endereco\":" + "\"" + item.First().ENDERECO + "\",";
                    js += "\"numero\":" + "\"" + item.First().NUMERO + "\",";
                    js += "\"longitude\":" + "\"" + item.First().LONGITUDE + "\",";
                    js += "\"latitude\":" + "\"" + item.First().LATITUDE + "\",";
                    js += "\"imagem\":" + "\"" + item.First().IMAGEM + "\",";
                    js += "\"tipo_estabelecimento\":" + "\"" + item.First().TIPO_ESTABELECIMENTO + "\",";
                    js += "\"descricao\":" + "\"" + item.First().DESCRICAO + "\",";
                    js += "\"imagem_promocao\":" + "\"" + item.First().IMAGEM_PROMOCAO + "\",";
                    js += "\"validade\":" + item.First().VALIDADE + ",";
                    js += "\"distancia_km\":" + item.First().DISTANCIA_KM + ",";
                    js += "\"desconto\":" + item.First().DESCONTO + ",";


                    js += "\"itens\":[";
                    int loopItens = 0;
                    foreach (Promocoes i in item)
                    {
                        if (loopItens > 0)
                            js += ",";

                        js += "{";
                        js += "\"codigo_item\":" + i.CODIGO_ITEM + ",";
                        js += "\"titulo\":" + "\"" + i.TITULO + "\",";
                        js += "\"descricao\":" + "\"" + i.DESCRICAO_ITEM + "\",";
                        js += "\"valor\":" + i.VALOR + ",";
                        js += "\"tempo_estimado_min\":" + i.TEMPO_ESPERADO_MIN + ",";
                        js += "\"tempo_estimado_max\":" + i.TEMPO_ESPERADO_MAX + ",";
                        js += "\"imagem\":" + "\"" + i.IMAGEM_PROMOCAO + "\",";
                        js += "\"qtd_item\":" + i.QTD_ITEM;
                        js += "}";
                        loopItens += 1;

                    }
                    loopPromocoes += 1;
                    js += "]";
                    js += "}";

                }
                js += "]";
                js += "}";
                return js;

            }
        }

    }
}
