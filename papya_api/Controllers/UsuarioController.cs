using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using papya_api.DataProvider;
using papya_api.Models;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController:Controller
    {
        public IUsuarioDataProvider UsuarioDataProvider;

        
        public UsuarioController(IUsuarioDataProvider UsuarioDataProvider)
        {
            this.UsuarioDataProvider = UsuarioDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<Usuario>> Get()
        {
            return await this.UsuarioDataProvider.GetUsuarios();
        }

        [HttpGet("GetUsuarioSocial")]
        public async Task<IEnumerable<UsuarioSocial>> GetUsuarioSocial(float latitude, float longitude, float distanciaKm, int? qtdLista, int? idEstabelecimento)
        {
            return await this.UsuarioDataProvider.GetUsuariosSocial(latitude, longitude,distanciaKm,qtdLista, idEstabelecimento);
        }

        //teste s

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<Usuario> Get(int id)
        {
            return await this.UsuarioDataProvider.GetUsuario(id);
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        //[Route("[action]/{cpf}")]
        [HttpGet("GetByCpf")]
        public async Task<Usuario> GetByCpf(string cpf)
        {
            return await this.UsuarioDataProvider.GetUsuarioCpf(cpf);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public object Post([FromBody] Usuario usuario,
                                [FromServices] DataProvider.UsuarioDAO usersDAO)
        {
            object a = "";
            var usuAuxCpf = usersDAO.ValidaCpf(usuario.Cpf);
            var usuAuxTelefone = usersDAO.ValidaTelefone(usuario.Telefone);
            var usuAuxEmail = usersDAO.ValidaEmail(usuario.Email);
            if (usuAuxCpf == null && usuAuxTelefone==null && usuAuxEmail==null)
            {
                a =this.UsuarioDataProvider.AddUsuario(usuario);
                if (Convert.ToBoolean(a) != true)
                {
                    a = StatusCode(500, new { retorno = "Erro ao tentar inserir" });
                }
                else
                {
                    a = StatusCode(200, new { retorno = "Dados inseridos com sucesso" });
                }
            }
            else {
                string msg = "";
                if (usuAuxCpf != null)
                {
                    msg = "Cpf já cadastrado";
                }
                else if (usuAuxTelefone != null)
                {
                    msg = "Telefone já cadastrado";
                }
                else if (usuAuxEmail != null)
                {
                    msg = "E-mail já cadastrado";
                }

                a = StatusCode(406, new { retorno = msg });
            }
            return a;
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        //[HttpPut("{id}")]
        [HttpPut]
        public object Put([FromBody] Usuario usuario)
        {
            Task<int?> a;
            object retorno = "";
            try
            {
                if (usuario.Resetar_Senha == null || usuario.Resetar_Senha == 0)
                {
                    a = this.UsuarioDataProvider.UpdateUsuario(usuario);
                }
                else
                {
                    a= this.UsuarioDataProvider.EsqueceuSenha(usuario.Email);
                }

                if (usuario.Resetar_Senha !=null && a.IsCompletedSuccessfully==true && a.Result.ToString().Equals("0"))
                {
                    retorno = StatusCode(406, new { retorno = "E-mail não cadastrado" });
                    //a.Status = TaskStatus.RanToCompletion;
                }
                else if (usuario.Resetar_Senha != null && a.IsFaulted==true)
                {
                    retorno = StatusCode(500, new { retorno = "Erro ao enviar o e-mail" });
                }else if(a.IsFaulted == true)
                {
                    retorno = StatusCode(500, new { retorno = a.Exception.Message });
                }
            }
            catch (System.Exception ex)
            {
                retorno = StatusCode(500, new { retorno = ex.Message });
            }

            return retorno;
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        //[HttpPut("{id}")]
        [HttpPost("UpdateStatus")]
        public object UpdateStatus([FromBody] Usuario usuario)
        {
            object retorno = "";
            try
            {
                retorno = this.UsuarioDataProvider.UpdateStatusUsuario(usuario);
            }
            catch (System.Exception ex)
            {
                retorno = ex;
            }

            return retorno;
        }

        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.UsuarioDataProvider.DeleteUsuario(id);
        }
    }
}
