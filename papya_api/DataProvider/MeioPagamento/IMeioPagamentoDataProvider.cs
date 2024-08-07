using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IMeioPagamentoDataProvider
    {
        Task<IEnumerable<MeioPagamento>> GetMeioPagamentos();

        Task<MeioPagamento> GetMeioPagamento(int idMeioPagamento);

        Task AddMeioPagamento(MeioPagamento meioPagamento);

        Task UpdateMeioPagamento(MeioPagamento meioPagamento);

        Task DeleteMeioPagamento(int idMeioPagamento);

    }
}
