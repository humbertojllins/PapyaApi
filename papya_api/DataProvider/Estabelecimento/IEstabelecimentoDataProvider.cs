using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IEstabelecimentoDataProvider
    {
        object GetEstabelecimentos(float? latitude, float? longitude, int? qtdLista, int? idTipoEstabelecimento);

        Task<Estabelecimento> GetEstabelecimento(int ididEstabelecimento);

        Task<Estabelecimento> GetEstabelecimentoCnpj(string cnpj);

        object AddEstabelecimento(Estabelecimento estabelecimento);

        Task UpdateEstabelecimento(Estabelecimento estabelecimento);

        Task DeleteEstabelecimento(int CodEstabelecimento);

    }
}
