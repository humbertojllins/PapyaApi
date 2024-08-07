using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface ICategoriaDataProvider
    {
        Task<IEnumerable<Categoria>> GetCategorias(int idEstabelecimento, int? qtdLista);

        Task<Categoria> GetCategoria();

        Task AddCategoria(Categoria categoria);

        Task UpdateCategoria(Categoria categoria);

        Task DeleteCategoria(int CodCategoria);

    }
}
