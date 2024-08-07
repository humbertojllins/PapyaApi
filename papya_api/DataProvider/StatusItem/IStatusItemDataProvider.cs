using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IStatusItemDataProvider
    {
        Task<IEnumerable<StatusItem>> GetStatusItems();

        Task<StatusItem> GetStatusItem(int CodStatusItem);

        Task AddStatusItem(StatusItem StatusItem);

        Task UpdateStatusItem(StatusItem StatusItem);

        Task DeleteStatusItem(int CodStatusItem);

    }
}
