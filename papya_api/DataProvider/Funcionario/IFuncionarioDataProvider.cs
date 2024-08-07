using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IFuncionarioDataProvider
    {
        Task<IEnumerable<object>> GetFuncionarios(int? id_estabelecimento);

        Task<Funcionario> GetFuncionario(int? idFuncionario);

        object AddFuncionario(int idEstabelecimento, int idUsuario, int id_tipo_funcionario);

        object ValidaCadastroFuncionario(int? idEstabelecimento, int? idUsuario);

        object UpdateFuncionario(Funcionario Funcionario);

        object DeleteFuncionario(int idFuncionario);

    }
}
