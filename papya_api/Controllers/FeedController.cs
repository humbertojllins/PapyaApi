using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using papya_api.DataProvider;
using papya_api.Models;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : Controller
    {
        public IPromocoesDataProvider PromocoesDataProvider;
        public ITipoEstabelecimentoDataProvider TipoEstabelecimentoDataProvider;
        public IEstabelecimentoDataProvider EstabelecimentoDataProvider;


        public FeedController(IPromocoesDataProvider PromocoesDataProvider, 
            ITipoEstabelecimentoDataProvider TipoEstabelecimentoDataProvider,
            IEstabelecimentoDataProvider EstabelecimentoDataProvider
            )
        {
            this.PromocoesDataProvider = PromocoesDataProvider;
            this.TipoEstabelecimentoDataProvider = TipoEstabelecimentoDataProvider;
            this.EstabelecimentoDataProvider = EstabelecimentoDataProvider;
        }

        //[Authorize("Bearer")]
        //[HttpGet("{id}")]
        [HttpGet]
        public object Get(float latitude, float longitude, int? qtdLista, int? idEstabelecimento)
        {
            string promocoes = this.PromocoesDataProvider.GetPromocoes(latitude, longitude, qtdLista, idEstabelecimento).ToString();
            string tipo_estabelecimento = this.TipoEstabelecimentoDataProvider.GetTipoEstabelecimentos(qtdLista).ToString();
            string estabelecimento = this.EstabelecimentoDataProvider.GetEstabelecimentos(latitude, longitude, qtdLista,null).ToString();


            string ret = "{" +
                promocoes.Remove(promocoes.Length-1,1).Remove(0, 1) +
                "," +
                tipo_estabelecimento.Remove(tipo_estabelecimento.Length - 1, 1).Remove(0, 1) +
                "," +
                estabelecimento.Remove(estabelecimento.Length - 1, 1).Remove(0, 1) +
            "}";
           
            return ret;

            //return this.PromocoesDataProvider.GetPromocoes(latitude, longitude, qtdLista);
        }

    }
}
