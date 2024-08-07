using System;
using MySql.Data.MySqlClient;
using System.IO;
using Dapper;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using papya_api.ExtensionMethods;
using papya_api.Models;
using papya_api.Services;

namespace papya_api.DataProvider
{
    public class CardapioDAO
    {
        //private readonly IConfiguration _configuration;

        public static IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;
        Global util = new Global();
        public CardapioDAO(IHostingEnvironment environment , IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }
        
        public object Upload(int id, IFormFile files)
        {
            if (files.Length > 0)
            {
                try
                {
                    //Recupera os dados da imagem atual para deletar se for update
                    Cardapio cardapioDelImagem = ImagemAtual(id);

                    var caminhoImagem = util.UploadImage(cardapioDelImagem.IMAGEM, id, _environment.WebRootPath, Global.PathEntidade.Cardapio, files);

                    if (id != 0)
                    {
                        UpdateImagem(id, caminhoImagem);
                    }
                    return caminhoImagem;
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            else
            {
                return "Unsuccessful";
            }
        }
        public Cardapio ImagemAtual(int id)
        {
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            using (var conexao = new MySqlConnection(strcon))
            {
                var sql = "select " +
                   "i.id as codigo_item, " +
                   "i.titulo as item_titulo, " +
                   "i.descricao as item_descricao, " +
                   "i.valor as valor_item, " +
                   "i.tempo_estimado_min, " +
                   "i.tempo_estimado_max, " +
                   "i.imagem," +
                   "i.fk_categoria_id as item_id_categoria," +
                   "i.fk_id_estabelecimento," +
                   "i.is_cozinha," +
                   "i.is_cardapio " +
                   "from itens i " +
                    "where id =" + id;
               Cardapio varCardapio =
                 conexao.QueryFirstOrDefault<Cardapio>(sql);
                return varCardapio;

            }
        }
        public void UpdateImagem(int id, string imagem)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update itens set " +
                    (imagem == "" ? "" : "imagem ='" + imagem + "' ") +
                    "where id=" + id.ToString();
                conexao.Execute(sql
                , commandType: System.Data.CommandType.Text);
            }
        }
    }
}
