using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Amazon.SimpleEmail.Model;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using static papya_api.Controllers.ImageController;

namespace papya_api.Services
{
    public class Global
    {
        //public static IHostingEnvironment _environment;

        public enum PathEntidade 
        {
            Usuario,
            Cardapio,
            Promocao,
            Estabelecimento
        }


        public Global()
        {
            
        }

        //

        /// <summary>
        /// Otimizar as imagens cadastradas 
        /// </summary>
        /// <returns></returns>
        public String CompressImage(string _inputPath, string _outputPath, bool pequeno)
        {
            //WebImage wi = new WebImage();
            try
            {
                using (var image = new MagickImage(_inputPath))
                {
                    var geometry = new MagickGeometry
                    {
                        Height = pequeno == true ? 350 : 650,
                        Width = pequeno == true ? 300 : 600,
                        IgnoreAspectRatio = false,
                        Greater = true
                    };
                    image.Resize(geometry);
                    using (var stream = new MemoryStream())
                    {
                        image.Write(stream);
                        byte[] imageByte = stream.ToArray();
                        System.IO.File.WriteAllBytes(_outputPath, imageByte);
                        //File.WriteAllBytes(_outputPath, imageByte);

                        //Deleta o arquivo temporário
                        if (System.IO.File.Exists(_inputPath))
                        {
                            System.IO.File.Delete(_inputPath);
                        }

                        return _outputPath;
                    }
                }
            }
            catch (MagickException ex) // Catch Magick ExceptionErrors
            {
                throw ex;
            }
            catch (Exception ex) // Catch Exception Errors 
            {
                throw ex;
            }
        }

        //public String CompressImage(string _inputPath, string _outputPath)
        //{
        //    byte[] content = System.IO.File.ReadAllBytes(Server.MapPath("~/Image/") + "1.jpg");
        //    WebImage webImage = new WebImage(content);
        //    webImage.Resize(100, 100, true, false);
        //    content = webImage.GetBytes();
        //    return new FileContentResult(content, "image/jpg");
        //}
        /// <summary>
        /// Gera uma senha criptografada para o usuário
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string CalculateMD5Hash(string input)
        {
            // Calcular o Hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // Converter byte array para string hexadecimal
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string UploadImage(string imagemAtual, int idUsuario, string RootPath, PathEntidade entidade, IFormFile files)
        {
            //Recupera a os dados de imagem atual do usuário
            //Usuario usuDelImagem = usersDAO.ImagemAtual(idUsuario);
            string pentidade="";

            switch (entidade)
            {
                case PathEntidade.Cardapio :
                    pentidade = "/Uploads/Cardapio/";
                    break;
                case PathEntidade.Estabelecimento:
                    pentidade = "/Uploads/Estabelecimento/";
                    break;
                case PathEntidade.Promocao:
                    pentidade = "/Uploads/Promocoes/";
                    break;
                case PathEntidade.Usuario:
                    pentidade = "/Uploads/Usuario/";
                    break;
            }

            var path = RootPath + imagemAtual;

            //Deleta a imagem atual do usuario
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            //Deleta a imagem atual do usuario

            //Criar o diretório se não existir
            if (!Directory.Exists(RootPath + pentidade))
            {
                Directory.CreateDirectory(RootPath + pentidade);
            }
            using (Stream filestream = System.IO.File.Create(RootPath + pentidade + idUsuario.ToString() + files.FileName))
            {
                string tmpFileName = RootPath + pentidade + idUsuario.ToString() + "tmp" + files.FileName;
                string finalFileName = RootPath + pentidade + idUsuario.ToString() + files.FileName;
                files.CopyTo(filestream);
                filestream.Flush();

                //Reduzir o tamanho da imagem
                //util.CompressImage(tmpFileName, finalFileName,true);
               
                return  pentidade + idUsuario.ToString() + files.FileName;
            }
        }

    }
}
