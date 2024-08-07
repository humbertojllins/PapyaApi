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
    public class MeioPagamentoController : Controller
    {
        public IMeioPagamentoDataProvider MeioPagamentoDataProvider;

        public MeioPagamentoController(IMeioPagamentoDataProvider MeioPagamentoDataProvider)
        {
            this.MeioPagamentoDataProvider = MeioPagamentoDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<MeioPagamento>> Get()
        {
            return await this.MeioPagamentoDataProvider.GetMeioPagamentos();
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<MeioPagamento> Get(int id)
        {
            return await this.MeioPagamentoDataProvider.GetMeioPagamento(id);
        }

        // POST api/values
        [Authorize("Bearer")]
        [HttpPost]
        public async Task Post([FromBody] MeioPagamento MeioPagamento)
        {
            await this.MeioPagamentoDataProvider.AddMeioPagamento(MeioPagamento);
        }

        // PUT api/values/5
        [Authorize("Bearer")]
        [HttpPut]
        public async Task Put(int id, [FromBody] MeioPagamento MeioPagamento)
        {
            await this.MeioPagamentoDataProvider.UpdateMeioPagamento(MeioPagamento);
        }

        // DELETE api/values/5
        [Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.MeioPagamentoDataProvider.DeleteMeioPagamento(id);
        }

    }
}
