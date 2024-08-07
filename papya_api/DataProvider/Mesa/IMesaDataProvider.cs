using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IMesaDataProvider
    {
        Task<IEnumerable<Mesa>> GetMesas(int idEstabelecimento);

        Task<Mesa> GetMesa(int CodMesa);

        object AddMesa(Mesa Mesa);

        object UpdateMesa(Mesa Mesa);

        object DeleteMesa(int CodMesa);

    }
}
