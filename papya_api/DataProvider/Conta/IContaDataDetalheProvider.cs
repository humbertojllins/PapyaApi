using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IContaDetalheDataProvider
    {
        object GetDetalheContas(int id_conta);
        object GetDetalheContasNovo(int id_estabelecimento, int? idFuncionario, int? is_cozinha, int? status_conta, int? statusItem);
        
    }
}
