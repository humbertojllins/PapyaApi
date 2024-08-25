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
using Org.BouncyCastle.Ocsp;

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
                int? totalItens = listaPromocoes.First().TOTALITENS;


                var sql = "insert into promocoes (descricao, data_vigencia,validade,fk_id_estabelecimento,desconto,totalItens)" +
                    " values('" + descricao + "','" + data_vigencia + "','" + validade + "'," + fk_id_estabelecimento + "," + desconto + "," + totalItens + ");";
                sql += " select last_insert_id();";

                int id_promocao = Convert.ToInt32(conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text));
                sql = "";
                foreach (Promocoes item in listaPromocoes)
                {
                    sql += " insert into itens_promocao (id_promocao,id_item, qtd_item)";
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
                var tr = conexao.BeginTransaction();

                int idpromocao = listaPromocoes.First().ID_PROMOCAO;
                string descricao = listaPromocoes.First().DESCRICAO;
                string data_vigencia = listaPromocoes.First().DATA_VIGENCIA;
                int validade = listaPromocoes.First().VALIDADE;
                int? fk_id_estabelecimento = listaPromocoes.First().ID_ESTABELECIMENTO;
                float desconto = listaPromocoes.First().DESCONTO;
                int? totalItens = listaPromocoes.First().TOTALITENS;
                var sql = "";

                try
                {
                    sql = "update promocoes set " +
                    "descricao='" + descricao + "'," +
                    "data_vigencia='" + data_vigencia + "'," +
                    "validade=" + validade + "," +
                    "desconto=" + desconto + "," +
                    "totalItens=" + totalItens +
                    " where id=" + idpromocao.ToString();

                    conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
                }
                catch (Exception ex)
                {   
                    return ex;
                }

                try
                {
                    sql = " delete from itens_promocao where id_promocao=" + idpromocao + ";";
                    conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
                }
                catch (Exception ex)
                {                    
                    return ex;
                    
                }
                try
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
                    tr.Commit();
                }
                catch (Exception ex)
                {   
                    return ex;
                }

                return true;
                //return ((Task<int>)ret).IsCompletedSuccessfully;
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
        public object AtivaPromocao(int idPromocao)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update promocoes SET " +
                    "status=1 " +
                    " where id=" + idPromocao.ToString();
                return conexao.Execute(sql, commandType: System.Data.CommandType.Text);
            }
        }
        
        public object GetPromocoes(float latitude, float longitude, int? qtdLista, int? idestabelecimento, int? status = 1)
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
                        "p.desconto, " +
                        "p.status, " +
                        "p.totalItens " +
                        " from tingledb.estabelecimento e " +
                    "inner join tingledb.tipo_estabelecimento tp on tp.id = e.fk_tipo_estabelecimento_id " +
                    "left join tingledb.promocoes p on e.id_estabelecimento = p.fk_id_estabelecimento";
                //sql += " where p.status=1 ";
                if (status == 1)
                {
                    sql += " where p.status=1 ";
                }
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
                    js += "\"data_vigencia\":" + "\"" + item.First().DATA_VIGENCIA + "\",";
                    js += "\"validade\":" + item.First().VALIDADE + ",";
                    js += "\"distancia_km\":" + item.First().DISTANCIA_KM + ",";
                    js += "\"desconto\":" + item.First().DESCONTO + ",";
                    js += "\"status\":" + item.First().STATUS + ",";
                    js += "\"totalItens\":" + item.First().TOTALITENS + ",";


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

        public object GetPromocoes(int id)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                var sql = "select " +
                    "p.fk_id_estabelecimento, " +
                    "p.id id_promocao," +
                    "p.descricao, " +
                    "p.data_vigencia, " +
                    "p.imagem as imagem_promocao, " +
                    "p.validade, " +
                    "p.desconto, " +
                    "p.totalItens, " +
                    "p.status, " +
                    "i.id as codigo_item, " +
                    "i.titulo, " +
                    "i.descricao as descricao_item, " +
                    "i.valor, " +
                    "i.tempo_estimado_min, " +
                    "i.tempo_estimado_max, " +
                    "i.imagem as imagem_item, " +
                    "ip.qtd_item" +
                    " from promocoes p " +
                    "inner join tingledb.itens_promocao ip on ip.id_promocao = p.id " +
                    "inner join tingledb.itens i on ip.id_item = i.id " +
                    "where p.id = " + id.ToString() +
                    " order by p.descricao;";

                IEnumerable<Promocoes> x = conexao.QueryAsync<Promocoes>(sql,
                null,
                commandType: System.Data.CommandType.Text).Result;

                //string js = "[";
                string js = "";
                //var listaPromo = from a in x
                //                 group a by a.ID_PROMOCAO into newGroup
                //                 orderby newGroup.Key
                //                 select newGroup;
                //int qtdPromo = 1;

                foreach (var item in x)
                {
                    js += "{";
                    js += "\"id_promocao\":" + item.ID_PROMOCAO + ",";
                    js += "\"id_estabelecimento\":" + item.ID_ESTABELECIMENTO + ",";
                    js += "\"nome\":" + "\"" + item.NOME + "\",";
                    js += "\"cnpj\":" + "\"" + item.CNPJ + "\",";
                    js += "\"endereco\":" + "\"" + item.ENDERECO + "\",";
                    js += "\"numero\":" + "\"" + item.NUMERO + "\",";
                    js += "\"longitude\":" + "\"" + item.LONGITUDE + "\",";
                    js += "\"latitude\":" + "\"" + item.LATITUDE + "\",";
                    js += "\"imagem\":" + "\"" + item.IMAGEM + "\",";
                    js += "\"tipo_estabelecimento   \":" + "\"" + item.TIPO_ESTABELECIMENTO + "\",";
                    js += "\"descricao\":" + "\"" + item.DESCRICAO + "\",";
                    js += "\"imagem_promocao\":" + "\"" + item.IMAGEM_PROMOCAO + "\",";
                    js += "\"data_vigencia\":" + "\"" + item.DATA_VIGENCIA + "\",";
                    js += "\"validade\":" + item.VALIDADE + ",";
                    js += "\"distancia_km\":" + item.DISTANCIA_KM + ",";
                    js += "\"desconto\":" + item.DESCONTO + ",";
                    js += "\"status\":" + item.STATUS + ",";
                    js += "\"totalItens\":" + item.TOTALITENS + ",";

                    break;
                }
                js += "\"itens\":[";
                int loopItens = 0;
                foreach (Promocoes i in x)
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
                js += "]";
                js += "}";
                //js += "]";
                return js;
            }
        }
    }
}
