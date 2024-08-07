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
    public class TipoFuncionarioController : Controller
    {
        public ITipoFuncionarioDataProvider tipoFuncionarioDataProvider;

        public TipoFuncionarioController(ITipoFuncionarioDataProvider tipoFuncionarioDataProvider)
        {
            this.tipoFuncionarioDataProvider = tipoFuncionarioDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<TipoFuncionario>> Get()
        {
            return await this.tipoFuncionarioDataProvider.GetTipoFuncionarios();
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<TipoFuncionario> Get(int id)
        {
            return await this.tipoFuncionarioDataProvider.GetTipoFuncionario(id);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public async Task Post([FromBody] TipoFuncionario tipoFuncionario)
        {
            await this.tipoFuncionarioDataProvider.AddTipoFuncionario(tipoFuncionario);
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        [HttpPut]
        public async Task Put(int id, [FromBody] TipoFuncionario tipoFuncionario)
        {
            await this.tipoFuncionarioDataProvider.UpdateTipoFuncionario(tipoFuncionario);
        }

        // DELETE api/values/5
        [Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.tipoFuncionarioDataProvider.DeleteTipoFuncionario(id);
        }

    }
}
