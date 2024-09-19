using Microsoft.AspNetCore.Mvc;
using papya_api.DataProvider;
using papya_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssinaturaController : Controller
    {
        public IAssinaturaDataProvider AssinaturaDataProvider;

        public AssinaturaController(IAssinaturaDataProvider assinaturaDataProvider)
        {
            this.AssinaturaDataProvider = assinaturaDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<Assinatura>> Get(int idEstabelecimento)
        {
            return await this.AssinaturaDataProvider.GetAssinaturas(idEstabelecimento);
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<Assinatura> Get1(int id)
        {
            return await this.AssinaturaDataProvider.GetAssinatura(id);
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{chave_pagamento}")]
        public async Task<Assinatura> GetByChave(string chave_pagamento)
        {
            return await this.AssinaturaDataProvider.GetAssinatura(chave_pagamento);
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("GetUltimaAssinatura")]
        public async Task<Assinatura> GetUltimaAssinatura(int idEstabelecimento)
        {
            return await this.AssinaturaDataProvider.GetUltimaAssinatura(idEstabelecimento);
        }
        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("GetAssinaturaPendente")]
        public async Task<Assinatura> GetAssinaturaPendente(int idEstabelecimento)
        {
            return await this.AssinaturaDataProvider.GetAssinaturaPendente(idEstabelecimento);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public object Post([FromBody] Assinatura assinatura)
        {
            return this.AssinaturaDataProvider.AddAssinatura(assinatura);
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        [HttpPut]
        public object Put([FromBody] Assinatura assinatura)
        {
            return this.AssinaturaDataProvider.UpdateAssinatura(assinatura);
        }

        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            return this.AssinaturaDataProvider.DeleteAssinatura(id);
        }
    }
}
