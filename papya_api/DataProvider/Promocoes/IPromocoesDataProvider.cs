using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IPromocoesDataProvider
    {
        object GetPromocoes(float latitude, float longitude, int? qtdLista, int? idestabelecimento);
        object AddPromocao(IEnumerable<Promocoes> listaPromocoes);
        object UpdatePromocao(IEnumerable<Promocoes> listaPromocoes);
        object DeletePromocao(int idPromocao);
    }
}
