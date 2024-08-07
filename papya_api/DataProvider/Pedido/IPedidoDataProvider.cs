using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IPedidoDataProvider
    {
        object GetPedidos(int idEstabelecimento);

        object AddPedido (IEnumerable<Pedido> listaPedido);

        Task DeletePedido(int item);

    }
}
