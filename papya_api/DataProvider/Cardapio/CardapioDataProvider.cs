﻿using System;
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
    public class CardapioDataProvider:ICardapioDataProvider
    {

        private readonly IConfiguration _configuration;

        public CardapioDataProvider()
        {

        }

        public CardapioDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object AddCardapio(string titulo, string descricao, double valor, int tempo_estimado_min, int tempo_estimado_max, int fk_categoria_item,int fk_id_estabelecimento, int is_cozinha, int is_cardapio)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "insert into itens (titulo,descricao,valor,tempo_estimado_min,tempo_estimado_max,";
                sql+= "fk_categoria_id,fk_id_estabelecimento,is_cozinha,is_cardapio)";
                sql += " values(" +
                    "'" + titulo + "'," +
                    "'" + descricao + "'," +
                    "" + valor + "," +
                    "" + tempo_estimado_min + "," +
                    "" + tempo_estimado_max + "," +
                    "" + fk_categoria_item + "," +
                    "" + fk_id_estabelecimento + "," +
                    "" + is_cozinha + "," +
                    "" + is_cardapio + ");";

                sql += " select last_insert_id();";

                return conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
                
                //return conexao.ExecuteAsync(sql, commandType: System.Data.CommandType.Text);
            }
        }

        public object DeleteCardapio(int id)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update itens set is_cardapio=0 " +
                    " where id=" + id.ToString();

                return conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);
            }
        }
        public  object UpdateCardapio(int id, string titulo, string descricao, double valor, int tempo_estimado_min, int tempo_estimado_max, int fk_categoria_item, int fk_id_estabelecimento, int is_cozinha, int is_cardapio)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update itens set " +
                    "titulo='" + titulo + "'," +
                    "descricao='" + descricao + "'," +
                    "valor=" + valor + "," +
                    "tempo_estimado_min=" + tempo_estimado_min + "," +
                    "tempo_estimado_max=" + tempo_estimado_max + "," +
                    "fk_categoria_id=" + fk_categoria_item + "," +
                    "is_cozinha=" + is_cozinha + "," +
                    "is_cardapio=" + is_cardapio +
                    " where id=" + id.ToString();
                    
                return conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);
            }
        }

        public object GetCardapios(int idEstabelecimento, int is_cardapio = 1)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                 conexao.Open();

                var sql = "select " +
                    "i.id as codigo_item, " +
                    "i.titulo as item_titulo, " +
                    "i.descricao as item_descricao, " +
                    "i.valor as valor_item, " +
                    "i.tempo_estimado_min, " +
                    "i.tempo_estimado_max, " +
                    "i.imagem, " +
                    "i.is_cozinha, " +
                    "i.is_cardapio, " +
                    "c.descricao as categoria " +
                    "from itens i " +
                    "left join categoria c on c.id = i.fk_categoria_id " +
                    "where i.fk_id_estabelecimento = " + idEstabelecimento.ToString();
                    if(is_cardapio==1) {
                        sql += " and is_cardapio=1";
                    }
                    sql+= " order by c.descricao;";

                IEnumerable<Cardapio> x = conexao.QueryAsync<Cardapio>(sql,
                null,
                commandType: System.Data.CommandType.Text).Result;
                string js= "[";
                var listaCateg = from a in x
                                          group a by a.categoria into newGroup
                                          orderby newGroup.Key
                                          select newGroup;
                int qtdCateg = 1;

                foreach (var item in listaCateg)
                {
                    js += "{";
                    js += "\"categoria\":";
                    //js += @"""" + item.Key + @"""itens:[";
                    //js += @"""" + item.Key + @""",";
                    js += "\"" + item.Key + "\",";
                    js += "\"itens\":[";
                    int loop = 0;
                    foreach (Cardapio c in x)
                    {
                        if (c.categoria == item.Key)
                        {
                            if (loop > 0)
                                js += ",";
                            js += " { ";
                            js += @"""codigo_item"":" + c.codigo_item + ",";
                            js += @"""titulo"":" + @"""" + c.item_titulo + @""",";
                            js += @"""item"":" + @"""" + c.item_descricao + @""",";
                            js += @"""valor"":" + c.valor_item + ",";
                            js += @"""tempo_estimado_min"":" + c.tempo_estimado_min + ",";
                            js += @"""tempo_estimado_max"":" + c.tempo_estimado_max + ",";
                            js += @"""imagem"":" + @"""" + c.imagem + @""",";
                            js += @"""is_cozinha"":" + @"""" + c.is_cozinha + @""",";
                            js += @"""is_cardapio"":" + @"""" + c.is_cardapio + @"""";
                            js += " }";
                            loop += 1;
                        }

                    }
                    js += " ]}";
                    loop = 0;
                    if (qtdCateg < listaCateg.Count())
                        js += ",";
                    qtdCateg += 1;
                }
                js += "]";

                //JArray array = new JArray();


                return js;
            }
        }
        public async Task<object> GetCardapio(int id)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();

                var sql = "select " +
                    "i.id as codigo_item, " +
                    "i.titulo as titulo, " +
                    "i.descricao as item, " +
                    "i.valor as valor, " +
                    "i.tempo_estimado_min, " +
                    "i.tempo_estimado_max, " +
                    "i.imagem, " +
                    "i.is_cozinha, " +
                    "i.fk_categoria_id as item_id_categoria, " +
                    "i.is_cardapio, " +
                    "c.descricao as categoria " +
                    "from itens i " +
                    "left join categoria c on c.id = i.fk_categoria_id " +
                    "where i.id = " + id.ToString() +
                    " order by c.descricao;";

                return await conexao.QuerySingleOrDefaultAsync<object>(sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

    }
}
