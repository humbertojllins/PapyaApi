using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using papya_api.DataProvider;
using papya_api.Models;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContaController:Controller
    {
        public IContaDataProvider ContaDataProvider;

        public ContaController(IContaDataProvider ContaDataProvider)
        {
            this.ContaDataProvider = ContaDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<IEnumerable<object>> Get(int id)
        {
            return await this.ContaDataProvider.GetContas(id);
        }
        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public object Post(int? idMesa, int? idUsuario, int? publico, int? idConta, char flagAbrirConta, char flagEntrarConta)
        {
            object a="";
            if (flagAbrirConta.ToString().ToUpper().Equals("X"))
            {
                 a = this.ContaDataProvider.AddConta(idMesa, idUsuario, publico);

                if (a.ToString().Contains("Retorno:406"))
                {
                    a = StatusCode(406, new { retorno = "Mesa ocupada ou Usuário possui conta aberta" });
                }
            }
            else if (flagEntrarConta.ToString().ToUpper().Equals("X"))
            {
                a = this.ContaDataProvider.AddContaUsuario(idConta, idUsuario, publico);

                if (a.ToString().Contains("Retorno:406"))
                {
                    a = StatusCode(406, new { retorno = "Usuário possui conta aberta" });
                }
            }
            else
            {
                return StatusCode(401);
            }

            return a;

        }

        [HttpPost("Fecharconta")]
        public object Fecharconta(int idConta, float total, int meioPagamento)
        {
            string mensagem = "";
            object a = "";
            bool valida = this.ContaDataProvider.ValidaFechamentoConta(out mensagem, idConta, total,true);
            if (valida == true)
            {
                a = this.ContaDataProvider.Fecharconta(idConta, total, meioPagamento);
            }
            else
            {
                a = StatusCode(406, new { retorno = mensagem });
            }
            return a;
        }

        [HttpPost("FecharcontaParcial")]
        public object FecharcontaParcial(int idConta, int idUsuarioConta, float total, int meioPagamento)
        {
            string mensagem = "";
            object a = "";
            bool valida = this.ContaDataProvider.ValidaFechamentoConta(out mensagem, idConta, total,false);
            if (valida == true)
            {
                if(mensagem=="FecharContaTotal") { 
                    a = this.ContaDataProvider.FecharcontaParcial(idConta, idUsuarioConta, total, meioPagamento,true);
                }
                else
                {
                    a = this.ContaDataProvider.FecharcontaParcial(idConta, idUsuarioConta, total, meioPagamento,false);
                }
            }
            else
            {
                a = StatusCode(406, new { retorno = mensagem });
            }
            return a;
        }

        [HttpPost("Pagarconta")]
        public object Pagarconta(int idUsuarioConta)
        {
            return this.ContaDataProvider.Pagarconta(idUsuarioConta);
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        //[HttpPut("{id}")]
        [HttpPut]
        public object Put([FromBody] Conta conta)
        {
            return this.ContaDataProvider.UpdateConta(conta);
        }

        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            return this.ContaDataProvider.DeleteConta(id);
        }
    }
}
