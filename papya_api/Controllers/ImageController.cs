using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using papya_api.Models;
using ImageMagick;
using papya_api.Services;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public static IHostingEnvironment _environment;
        Global util = new Global();
        public ImageController(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        public class FIleUploadAPI
        {
            public IFormFile files { get; set; }
        }
        [HttpPost]
        public object Post(
            [FromServices]DataProvider.UsuarioDAO usersDAO,
            [FromForm] int idUsuario, 
            [FromForm] FIleUploadAPI files)
        {
            if (files.files.Length > 0)
            {
                try
                {
                    //Recupera a os dados de imagem atual do usuário
                    Usuario usuDelImagem = usersDAO.ImagemAtual(idUsuario);

                    var path = _environment.WebRootPath + usuDelImagem.Imagem;

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    //Deleta a imagem atual do usuario
                    if (!Directory.Exists(_environment.WebRootPath + "/uploads/usuario/"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "/uploads/usuario/");
                    }
                    //using (Stream filestream = System.IO.File.Create(_environment.WebRootPath + "/uploads/usuario/" + idUsuario.ToString() + "tmp" + files.files.FileName))
                    using (Stream filestream = System.IO.File.Create(_environment.WebRootPath + "/uploads/usuario/" + idUsuario.ToString() + files.files.FileName))
                    {
                        string tmpFileName = _environment.WebRootPath + "/uploads/usuario/" + idUsuario.ToString() + "tmp" + files.files.FileName;
                        string finalFileName = _environment.WebRootPath + "/uploads/usuario/" + idUsuario.ToString() + files.files.FileName;
                        files.files.CopyTo(filestream);
                        filestream.Flush();

                        //Reduzir o tamanho da imagem
                        //util.CompressImage(tmpFileName, finalFileName,true);

                        if (idUsuario != 0)
                        {
                            usersDAO.UpdateUsuarioImagem(idUsuario, "/uploads/usuario/" + idUsuario.ToString() + files.files.FileName);
                        }
                        return "/uploads/usuario/" + idUsuario.ToString() + files.files.FileName;
                    }
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
    }
}
