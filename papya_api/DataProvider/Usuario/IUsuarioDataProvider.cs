using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;

namespace papya_api.DataProvider
{
    public interface IUsuarioDataProvider
    {
        Task<IEnumerable<Usuario>> GetUsuarios();

        Task<IEnumerable<UsuarioSocial>> GetUsuariosSocial(float latitude, float longitude, float distanciaKm, int? qtdLista, int? idEstabelecimento, int usuariologado);

        Task<Usuario> GetUsuario(int idUsuario);

        object GetUsuarioMesa(int idUsuarioConta);

        Task<Usuario> GetUsuarioCpf(string cpf);

        object AddUsuario(Usuario usuario);

        Task<int?> UpdateUsuario(Usuario usuario);

        object UpdateStatusUsuario(Usuario usuario);

        object UpdateChaveNotificacaoUsuario(Usuario usuario);

        Task DeleteUsuario(int idUsuario);

        Task<int?> EsqueceuSenha(string email);

        Task<string> ValidaEmailUsuario(string chaveDinamica);


    }
}
