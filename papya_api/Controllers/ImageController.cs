using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using papya_api.Models;
using ImageMagick;
using papya_api.Services;
using papya_api.DataProvider;

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
            [FromServices] DataProvider.UsuarioDAO usersDAO,
            [FromForm] int idUsuario,
            [FromForm] FIleUploadAPI files)
        {
            if (files.files.Length > 0)
            {
                try
                {
                    //Recupera a os dados de imagem atual do usuário
                    Usuario estDelImagem = usersDAO.ImagemAtual(idUsuario);
                    var caminhoImagem = util.UploadImage(estDelImagem.Imagem, idUsuario, _environment.WebRootPath, Global.PathEntidade.Usuario, files.files);

                    if (idUsuario != 0)
                    {
                        usersDAO.UpdateUsuarioImagem(idUsuario, caminhoImagem);
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
    }
}
