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
    public class LoginController : Controller
    {

        [AllowAnonymous]
        [HttpPost]
        public object Post(
            [FromBody]Usuario usuario,
            [FromServices]DataProvider.UsuarioDAO usersDAO,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {

            try
            {
         
                bool credenciaisValidas = false;
                if (usuario != null && !String.IsNullOrWhiteSpace(usuario.Telefone))
                {
                    Usuario usuarioBase = usersDAO.Find(usuario.Telefone, usuario.Senha);
                    credenciaisValidas = (usuarioBase != null && usuario.Telefone == usuarioBase.Telefone);
                    usuario = usuarioBase;
                }

                if (credenciaisValidas)
                {
                    ClaimsIdentity identity = new ClaimsIdentity(
                        new GenericIdentity(usuario.Id.ToString(), "Id"),
                        new[] {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                            new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Telefone)
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
                        id=usuario.Id,
                        Id_Tipo_Usuario = usuario.Id_Tipo_Usuario,
                        Nome=usuario.Nome,
                        Cpf=usuario.Cpf,
                        Nascimento = usuario.Nascimento,
                        Login=usuario.Login,
                        Email = usuario.Email,
                        Imagem = usuario.Imagem,
                        Resetar_Senha = usuario.Resetar_Senha,
                        Telefone = usuario.Telefone,
                        StatusSocial = usuario.StatusSocial
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
