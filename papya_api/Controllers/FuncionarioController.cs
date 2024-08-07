using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using papya_api.DataProvider;
using papya_api.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionarioController : Controller
    {
        public IFuncionarioDataProvider FuncionarioDataProvider;

        public FuncionarioController(IFuncionarioDataProvider funcionarioDataProvider)
        {
            this.FuncionarioDataProvider = funcionarioDataProvider;
        }
        
        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<object>> Get(int? idEstabelecimento)
        {
            return await this.FuncionarioDataProvider.GetFuncionarios(idEstabelecimento);
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<Funcionario> Get(int id)
        {
            return await this.FuncionarioDataProvider.GetFuncionario(id);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public object Post([FromBody] Funcionario funcionario)
        {
            var retorno = this.FuncionarioDataProvider.AddFuncionario(funcionario.ID_ESTABELECIMENTO, funcionario.ID_USUARIO, funcionario.ID_TIPOFUNCIONARIO);
            if(retorno.ToString().Contains("406"))
            {
                return StatusCode(406, "Funcionário já cadastrado" );
            }
            return retorno;
        }

        // PUT api/values/5
        [Authorize("Bearer")]
        [HttpPut]
        public object Put(int id, [FromBody] Funcionario Funcionario)
        {
            return this.FuncionarioDataProvider.UpdateFuncionario(Funcionario);
        }

        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            return this.FuncionarioDataProvider.DeleteFuncionario(id);
        }
    }
}
