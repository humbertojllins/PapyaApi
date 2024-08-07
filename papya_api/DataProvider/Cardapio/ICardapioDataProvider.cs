using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface ICardapioDataProvider
    {
        object GetCardapios(int idEstabelecimento);

        object AddCardapio(string titulo, string descricao, double valor, int tempo_estimado_min, int tempo_estimado_max,  int fk_categoria_item, int fk_id_estabelecimento, int is_cozinha, int is_cardapio);

        object UpdateCardapio(int id, string titulo, string descricao, double valor, int tempo_estimado_min, int tempo_estimado_max,  int fk_categoria_item, int fk_id_estabelecimento, int is_cozinha, int is_cardapio);

        object DeleteCardapio(int item);

    }
}
