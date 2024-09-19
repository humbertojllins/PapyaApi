using System;
using MySql.Data.MySqlClient;
using System.IO;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using papya_api.ExtensionMethods;
using papya_api.Models;
using papya_api.Services;

namespace papya_api.DataProvider
{
    public class PromocoesDAO
    {
        //private readonly IConfiguration _configuration;

        public static IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;
        Global util = new Global();
        public PromocoesDAO(IHostingEnvironment environment , IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        //public object Upload(int id, IFormFile files)
        //{
        //    if (files.Length > 0)
        //    {
        //        try
        //        {
        //            //Recupera os dados da imagem atual para deletar se for update
        //            Promocoes promocoesDelImagem = ImagemAtual(id);

        //            var path = _environment.WebRootPath + promocoesDelImagem.IMAGEM;

        //            if (System.IO.File.Exists(path))
        //            {
        //                System.IO.File.Delete(path);
        //            }
        //            //Deleta a imagem atual do cardápio

        //            if (!Directory.Exists(_environment.WebRootPath + "/uploads/Promocoes/"))
        //            {
        //                Directory.CreateDirectory(_environment.WebRootPath + "/uploads/Promocoes/");
        //            }
        //            using (FileStream filestream = System.IO.File.Create(_environment.WebRootPath + "/uploads/Promocoes/" + id.ToString() + "tmp" + files.FileName))
        //            {
        //                string tmpFileName = _environment.WebRootPath + "/uploads/Promocoes/" + id.ToString() + "tmp" + files.FileName;
        //                string finalFileName = _environment.WebRootPath + "/uploads/Promocoes/" + id.ToString() + files.FileName;
        //                files.CopyTo(filestream);
        //                filestream.Flush();

        //                //Reduzir o tamanho da imagem
        //                util.CompressImage(tmpFileName, finalFileName, false);

        //                if (id != 0)
        //                {
        //                    UpdateImagem(id, "/uploads/Promocoes/" + id.ToString() + files.FileName);
        //                }
        //                return "/uploads/Promocoes/" + id.ToString() + files.FileName;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.ToString();
        //        }
        //    }
        //    else
        //    {
        //        return "Unsuccessful";
        //    }
        //}

        public object Upload(int id, IFormFile files)
        {
            if (files.Length > 0)
            {
                try
                {
                    //Recupera os dados da imagem atual para deletar se for update
                    Promocoes promocoesDelImagem = ImagemAtual(id);

                    var caminhoImagem = util.UploadImage(promocoesDelImagem.IMAGEM, id, _configuration.GetSection("imageURL:PATH").Value, Global.PathEntidade.Promocao, files);

                    //    //Reduzir o tamanho da imagem
                    //    util.CompressImage(tmpFileName, finalFileName, false);

                    if (id != 0)
                    {
                        UpdateImagem(id, caminhoImagem);
                    }
                    return caminhoImagem;
                    //}
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



        public Promocoes ImagemAtual(int id)
        {
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            using (var conexao = new MySqlConnection(strcon))
            {
                var sql = "select " +
                   "p.imagem " +
                   "from promocoes p " +
                    "where id =" + id;
                Promocoes varPromocoes =
                 conexao.QueryFirstOrDefault<Promocoes>(sql);
                return varPromocoes;

            }
        }
        public void UpdateImagem(int id, string imagem)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update promocoes set " +
                    (imagem == "" ? "" : "imagem ='" + imagem + "' ") +
                    "where id=" + id.ToString();
                conexao.Execute(sql
                , commandType: System.Data.CommandType.Text);
            }
        }
    }
}
