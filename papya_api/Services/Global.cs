using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
//using Amazon.SimpleEmail.Model;
using Google.Apis.Auth.OAuth2;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using papya_api.Models;
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
            string tmpFileName = RootPath + pentidade + idUsuario.ToString() + "tmp" + files.FileName;
            string finalFileName = RootPath + pentidade + idUsuario.ToString() + files.FileName;
            //using (Stream filestream = System.IO.File.Create(_environment.WebRootPath + "/uploads/usuario/" + idUsuario.ToString() + "tmp" + files.files.FileName))
            //using (Stream filestream = System.IO.File.Create(RootPath + pentidade + idUsuario.ToString() + "tmp" + files.FileName))
            using (Stream filestream = System.IO.File.Create(tmpFileName))
            {
                //string tmpFileName = RootPath + pentidade + idUsuario.ToString() + "tmp" + files.FileName;
                //string finalFileName = RootPath + pentidade + idUsuario.ToString() + files.FileName;
                files.CopyTo(filestream);
                filestream.Flush();
            }
            //Reduzir o tamanho da imagem
            CompressImage(tmpFileName, finalFileName, true);
            return pentidade + idUsuario.ToString() + files.FileName;
        }

        public void EnviarNotificacaoFirebase(string pathProjeto, string chaveUsuario, string titulo, string body,string image, string key1, string key2)
        {
            //----------Generating Bearer token for FCM---------------
            string contentRootPath = pathProjeto;
            string fileName = contentRootPath + "/papyanotificacao-718bfc509b1e.json";
            string scopes = "https://www.googleapis.com/auth/firebase.messaging";
            var bearertoken = ""; // Bearer Token in this variable

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                bearertoken = GoogleCredential
                  .FromStream(stream) // Loads key file
                  .CreateScoped(scopes) // Gathers scopes requested
                  .UnderlyingCredential // Gets the credentials
                  .GetAccessTokenForRequestAsync().Result; // Gets the Access Token
            }

            ///--------Calling FCM-----------------------------

            var clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);

            client.BaseAddress = new Uri("https://fcm.googleapis.com/v1/projects/papyanotificacao/messages:send"); // FCM HttpV1 API

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearertoken); // Authorization Token in this variable

            //---------------Assigning Of data To Model --------------

            Root rootObj = new Root();
            rootObj.message = new Message();
            rootObj.message.token = chaveUsuario; //FCM Token id

            rootObj.message.data = new Data();
            rootObj.message.data.title = titulo;
            rootObj.message.data.body = body;
            rootObj.message.data.image = image;

            rootObj.message.data.key_1 = key1;
            rootObj.message.data.key_2 = key2;
            rootObj.message.notification = new Notification();
            rootObj.message.notification.title = titulo;
            rootObj.message.notification.body = body;
            rootObj.message.notification.image = image;

            //-------------Convert Model To JSON ----------------------
            var jsonObj = JsonConvert.SerializeObject(rootObj);

            //------------------------Calling Of FCM Notify API-------------------

            var data = new StringContent(jsonObj.ToString(), Encoding.UTF8, "application/json");
            data.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //var response = client.PostAsync("https://fcm.googleapis.com/v1/projects/haggnotificacao/messages:send", data).Result; // Calling The FCM httpv1 API
            var response = client.PostAsync("https://fcm.googleapis.com/v1/projects/papyanotificacao/messages:send", data).Result; // Calling The FCM httpv1 API
            //---------- Deserialize Json Response from API ----------------------------------

            var jsonResponse = response.Content.ReadAsStringAsync().Result;
            var responseObj = JsonConvert.DeserializeObject(jsonResponse);
        }

    }
}
