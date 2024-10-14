using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface ICategoriaDataProvider
    {
        Task<IEnumerable<Categoria>> GetCategorias(int idEstabelecimento, int? qtdLista);

        Task<Categoria> GetCategoria(int idCategoria);

        object AddCategoria(Categoria categoria);

        object UpdateCategoria(Categoria categoria);

    }
}
