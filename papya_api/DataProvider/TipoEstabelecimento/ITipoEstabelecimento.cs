using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface ITipoEstabelecimentoDataProvider
    {
        object GetTipoEstabelecimentos(int? qtdLista);

        Task<TipoEstabelecimento> GetTipoEstabelecimento(int id);

        Task AddTipoEstabelecimento(TipoEstabelecimento estabelecimento);

        Task UpdateTipoEstabelecimento(TipoEstabelecimento estabelecimento);

        Task DeleteTipoEstabelecimento(int CodTipoEstabelecimento);

    }
}
