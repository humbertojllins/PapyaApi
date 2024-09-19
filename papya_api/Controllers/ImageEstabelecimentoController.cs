using System;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using papya_api.DataProvider;
using papya_api.Models;
using papya_api.Services;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageEstabelecimentoController : ControllerBase
    {
        public static IHostingEnvironment _environment;
        Global util = new Global();
        public IConfiguration _configuration { get; }
        public ImageEstabelecimentoController(IHostingEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }
        public class FIleUploadAPI
        {
            public IFormFile files { get; set; }
        }
        [HttpPost]
        public async Task<string> Post(
            [FromServices]DataProvider.EstabelecimentoDAO estabelecimentoDAO,
            [FromForm] int idEstabelecimento, 
            [FromForm] FIleUploadAPI files)
        {
            if (files.files.Length > 0)
            {
                try
                {
                    //Recupera a os dados de imagem atual do usuário
                    Estabelecimento estDelImagem = estabelecimentoDAO.ImagemAtual(idEstabelecimento);
                    var caminhoImagem = util.UploadImage(estDelImagem.Imagem, idEstabelecimento, _configuration.GetSection("imageURL:PATH").Value, Global.PathEntidade.Estabelecimento, files.files);

                    if (idEstabelecimento != 0)
                    {
                        estabelecimentoDAO.UpdateEstabelecimentoImagem(idEstabelecimento,caminhoImagem);
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
