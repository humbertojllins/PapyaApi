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
    public class TipoEstabelecimentoController : Controller
    {
        public ITipoEstabelecimentoDataProvider tipoEstabelecimentoDataProvider;

        public TipoEstabelecimentoController(ITipoEstabelecimentoDataProvider tipoEstabelecimentoDataProvider)
        {
            this.tipoEstabelecimentoDataProvider = tipoEstabelecimentoDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public object Get(int? qtdLista)
        {
            return this.tipoEstabelecimentoDataProvider.GetTipoEstabelecimentos(qtdLista);
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<TipoEstabelecimento> Get(int id)
        {
            return await this.tipoEstabelecimentoDataProvider.GetTipoEstabelecimento(id);
        }

        // POST api/values
        [Authorize("Bearer")]
        [HttpPost]
        public async Task Post([FromBody] TipoEstabelecimento tipoEstabelecimento)
        {
            await this.tipoEstabelecimentoDataProvider.AddTipoEstabelecimento(tipoEstabelecimento);
        }

        // PUT api/values/5
        [Authorize("Bearer")]
        [HttpPut]
        public async Task Put(int id, [FromBody] TipoEstabelecimento tipoEstabelecimento)
        {
            await this.tipoEstabelecimentoDataProvider.UpdateTipoEstabelecimento(tipoEstabelecimento);
        }

        // DELETE api/values/5
        [Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.tipoEstabelecimentoDataProvider.DeleteTipoEstabelecimento(id);
        }

    }
}
