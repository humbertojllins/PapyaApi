using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface ITipoUsuarioDataProvider
    {
        Task<IEnumerable<TipoUsuario>> GetTipoUsuarios();

        Task<TipoUsuario> GetTipoUsuario(int CodTipoUsuario);

        Task AddTipoUsuario(TipoUsuario tipoUsuario);

        Task UpdateTipoUsuario(TipoUsuario tipoUsuario);

        Task DeleteTipoUsuario(int CodTipoUsuario);

    }
}
