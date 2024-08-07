using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface ITipoFuncionarioDataProvider
    {
        Task<IEnumerable<TipoFuncionario>> GetTipoFuncionarios();

        Task<TipoFuncionario> GetTipoFuncionario(int CodTipoFuncionario);

        Task AddTipoFuncionario(TipoFuncionario tipoFuncionario);

        Task UpdateTipoFuncionario(TipoFuncionario tipoFuncionario);

        Task DeleteTipoFuncionario(int CodTipoFuncionario);

    }
}
