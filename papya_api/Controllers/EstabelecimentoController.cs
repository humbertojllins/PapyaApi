using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using papya_api.Models;
using papya_api.DataProvider;
using Microsoft.AspNetCore.Authorization;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstabelecimentoController : Controller
    {
        public IEstabelecimentoDataProvider estabelecimentoDataProvider;

        public EstabelecimentoController(IEstabelecimentoDataProvider estabelecimentoDataProvider)
        {
            this.estabelecimentoDataProvider = estabelecimentoDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public object Get(float? latitude, float? longitude, int? qtdLista, int? idTipoEstabelecimento)
        {
            return this.estabelecimentoDataProvider.GetEstabelecimentos(latitude, longitude, qtdLista, idTipoEstabelecimento);
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<Estabelecimento> Get(int id)
        {
            return await this.estabelecimentoDataProvider.GetEstabelecimento(id);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public object Post([FromBody] Estabelecimento estabelecimento)
        {
            object a = "";
            var estab = this.estabelecimentoDataProvider.GetEstabelecimentoCnpj(estabelecimento.Cnpj);
            if (estab.Result == null)
            {
                a= this.estabelecimentoDataProvider.AddEstabelecimento(estabelecimento);
            }
            else {
                a= StatusCode(406, new { retorno = "Cnpj já cadastrado" });
            }
            return a;
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        [HttpPut]
        public async Task Put(int id, [FromBody] Estabelecimento estabelecimento)
        {
            await this.estabelecimentoDataProvider.UpdateEstabelecimento(estabelecimento);
        }

        // DELETE api/values/5
        [Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.estabelecimentoDataProvider.DeleteEstabelecimento(id);
        }

    }
}
