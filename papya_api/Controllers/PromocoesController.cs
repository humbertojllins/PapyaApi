using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using papya_api.DataProvider;
using papya_api.Models;
using papya_api.Services;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromocoesController : Controller
    {
        public IPromocoesDataProvider PromocoesDataProvider;
        public static IHostingEnvironment _environment;
        Global util = new Global();


        public ImageController img;

        public PromocoesController(IPromocoesDataProvider PromocoesDataProvider)
        {
            this.PromocoesDataProvider = PromocoesDataProvider;
        }

        //[Authorize("Bearer")]
        //
        [HttpGet]
        public object Get(float latitude, float longitude, int? qtdLista, int? idEstabelecimento)
        {
            return this.PromocoesDataProvider.GetPromocoes(latitude, longitude, qtdLista, idEstabelecimento);
        }

        [HttpPost]
        public object Post(
            IEnumerable<Promocoes> listaPromocoes,
            [FromForm] IFormFile files,
            [FromServices] DataProvider.PromocoesDAO promocoesDAO
            )
        {
            //img = new ImageController(_environment);
            var retorno= this.PromocoesDataProvider.AddPromocao(listaPromocoes);
            //return cardapioDAO.Upload(Convert.ToInt32(retorno), files);
            
            return StatusCode(200, new { idPromocao = retorno });
            
            //ImageController c = new ImageController(e);
        }

        [HttpPost("PostImagem")]
        public object PostImagem(
            [FromForm] int idPromocao,
            [FromForm] IFormFile files,
            [FromServices] DataProvider.PromocoesDAO promocoesDAO
            )
        {
            return promocoesDAO.Upload(idPromocao, files);

        }


        // PUT api/values/5
        //[Authorize("Bearer")]
        //[HttpPut("{id}")]
        [HttpPut]
        public object Put(
            IEnumerable<Promocoes> listaPromocoes,
            [FromForm] IFormFile files,
            [FromServices] DataProvider.PromocoesDAO promocoesDAO)
        {
            var ret = this.PromocoesDataProvider.UpdatePromocao(listaPromocoes);
            if (((Task<int>)ret).IsFaulted)
            {
                return StatusCode(500, new { retorno = "Erro ao atualizar" });
            }
            return promocoesDAO.Upload(listaPromocoes.First().ID_PROMOCAO, files);
        }

        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            return this.PromocoesDataProvider.DeletePromocao(id);
        }

        //[HttpPost]
        //public object Post(IEnumerable<Promocoes> listaPromocoes,
        //    [FromForm] IFormFile files,
        //    [FromServices] DataProvider.PromocoesDAO promocoesDAO
        //    )
        //{
        //    object retorno = this.PromocoesDataProvider.AddPromocao(listaPromocoes);
        //    return promocoesDAO.Upload(Convert.ToInt32(retorno), files);
        //}

    }
}
