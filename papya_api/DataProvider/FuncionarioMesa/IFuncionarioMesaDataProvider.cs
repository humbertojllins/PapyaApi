using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IFuncionarioMesaDataProvider
    {
        Task<IEnumerable<FuncionarioMesa>> GetFuncionarioMesas(int idMesa);

        object AddFuncionarioMesa(FuncionarioMesa FuncionarioMesa);

        //Task UpdateFuncionario(Funcionario Funcionario);

        int DeleteFuncionarioMesa(int IdFuncionarioMesa);

    }
}
