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
    public class FuncionarioMesaController : Controller
    {
        public IFuncionarioMesaDataProvider FuncionarioMesaDataProvider;

        public FuncionarioMesaController(IFuncionarioMesaDataProvider FuncionarioMesaDataProvider)
        {
            this.FuncionarioMesaDataProvider = FuncionarioMesaDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<IEnumerable<FuncionarioMesa>> Get(int id)
        {
            return await this.FuncionarioMesaDataProvider.GetFuncionarioMesas(id);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public object Post([FromBody] FuncionarioMesa FuncionarioMesa)
        {
            return this.FuncionarioMesaDataProvider.AddFuncionarioMesa(FuncionarioMesa);
        }

        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            int res = this.FuncionarioMesaDataProvider.DeleteFuncionarioMesa(id);
            if (res == 0)
            {
                return StatusCode(406, new { retorno = "Nenhum registro excluído" });
            }
            else
            {
                return StatusCode(200, new { retorno = "Ok" });
            }
        }

    }
}
