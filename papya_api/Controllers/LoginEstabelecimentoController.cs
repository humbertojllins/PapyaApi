using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using papya_api.Models;
using papya_api.DataProvider;


namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginEstabelecimentoController : Controller
    {

        [AllowAnonymous]
        [HttpPost]
        public object Post(
            [FromBody]Estabelecimento estabelecimento,
            [FromServices]DataProvider.EstabelecimentoDAO estabelecimentoDAO,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {

            try
            {
         
                bool credenciaisValidas = false;
                if (estabelecimento != null && !String.IsNullOrWhiteSpace(estabelecimento.Cnpj))
                {
                    Estabelecimento estabelecimentoBase = estabelecimentoDAO.Find(estabelecimento.Cnpj, estabelecimento.Senha);
                    credenciaisValidas = (estabelecimentoBase != null && estabelecimento.Cnpj == estabelecimentoBase.Cnpj);
                    estabelecimento = estabelecimentoBase;
                }

                if (credenciaisValidas)
                {
                    ClaimsIdentity identity = new ClaimsIdentity(
                        new GenericIdentity(estabelecimento.Id_Estabelecimento.ToString(), "Id_Estabelecimento"),
                        new[] {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                            new Claim(JwtRegisteredClaimNames.UniqueName, estabelecimento.Cnpj)
                        }
                    );

                    DateTime dataCriacao = DateTime.Now;
                    DateTime dataExpiracao = dataCriacao +  
                        TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                    var handler = new JwtSecurityTokenHandler();
                    var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                    {
                        Issuer = tokenConfigurations.Issuer,
                        Audience = tokenConfigurations.Audience,
                        SigningCredentials = signingConfigurations.SigningCredentials,
                        Subject = identity,
                        NotBefore = dataCriacao,
                        Expires = dataExpiracao
                    });
                    var token = handler.WriteToken(securityToken);

                    return new
                    {
                        authenticated = true,
                        created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                        expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                        accessToken = token,
                        message = "OK",
                        Id_Estabelecimento = estabelecimento.Id_Estabelecimento,
                        Nome = estabelecimento.Nome,
                        Cnpj=estabelecimento.Cnpj,
                        Endereco= estabelecimento.Endereco,
                        Numero = estabelecimento.Numero,
                        Latitude= estabelecimento.Latitude,
                        Longitude = estabelecimento.Longitude,
                        IdTipoEstabelecimento = estabelecimento.fk_Tipo_Estabelecimento_id,
                        Imagem = estabelecimento.Imagem,
                        Resetar_Senha = estabelecimento.Resetar_Senha
                    };
                }
                else
                {
                    return StatusCode(401);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
