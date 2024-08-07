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
    public class CategoriaController : Controller
    {
        public ICategoriaDataProvider categoriaDataProvider;

        public CategoriaController(ICategoriaDataProvider categoriaDataProvider)
        {
            this.categoriaDataProvider = categoriaDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<Categoria>> Get(int idEstabelecimento, int? qtdLista)
        {
            return await this.categoriaDataProvider.GetCategorias(idEstabelecimento, qtdLista);
        }

        // GET api/values/5
        [Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<Categoria> Get(int id)
        {
            return await this.categoriaDataProvider.GetCategoria();
        }

        // POST api/values
        [Authorize("Bearer")]
        [HttpPost]
        public async Task Post([FromBody] Categoria categoria)
        {
            await this.categoriaDataProvider.AddCategoria(categoria);
        }

        // PUT api/values/5
        [Authorize("Bearer")]
        [HttpPut]
        public async Task Put(int id, [FromBody] Categoria categoria)
        {
            await this.categoriaDataProvider.UpdateCategoria(categoria);
        }

        // DELETE api/values/5
        [Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.categoriaDataProvider.DeleteCategoria(id);
        }

    }
}
