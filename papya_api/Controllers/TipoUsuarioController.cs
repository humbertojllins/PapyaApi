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
    public class TipoUsuarioController : Controller
    {
        public ITipoUsuarioDataProvider tipoUsuarioDataProvider;

        public TipoUsuarioController(ITipoUsuarioDataProvider tipoUsuarioDataProvider)
        {
            this.tipoUsuarioDataProvider = tipoUsuarioDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<TipoUsuario>> Get()
        {
            return await this.tipoUsuarioDataProvider.GetTipoUsuarios();
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<TipoUsuario> Get(int id)
        {
            return await this.tipoUsuarioDataProvider.GetTipoUsuario(id);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public async Task Post([FromBody] TipoUsuario tipoUsuario)
        {
            await this.tipoUsuarioDataProvider.AddTipoUsuario(tipoUsuario);
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        [HttpPut]
        public async Task Put(int id, [FromBody] TipoUsuario tipoUsuario)
        {
            await this.tipoUsuarioDataProvider.UpdateTipoUsuario(tipoUsuario);
        }

        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.tipoUsuarioDataProvider.DeleteTipoUsuario(id);
        }

    }
}
