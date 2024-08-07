using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IContaDataProvider
    {
        Task<IEnumerable<object>> GetContas(int? id_usuario);

        Task<Conta> GetConta(int? idConta);

        object AddConta(int? idMesa, int? idUsuario, int? publico);

        object AddContaUsuario(int? idConta, int? idUsuario, int? publico);

        object ValidaAberturaConta(int? idMesa, int? idUsuario);

        object ValidaUsuarioEntrarConta(int? idUsuario);

        object UpdateConta(Conta conta);

        object DeleteConta(int idConta);

        object Pagarconta(int idUsuarioConta);

        object Fecharconta(int idConta, float total, int meioPagamento);

        object FecharcontaParcial(int idConta, int idUsuarioConta, float total, int meioPagamento, bool ultimoClienteMesa);

        bool ValidaFechamentoConta(out string mensagem, int idConta, float total, bool fechamentoTotal);

    }
}
