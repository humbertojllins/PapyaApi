using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;

namespace papya_api.DataProvider
{
    public interface IAssinaturaDataProvider
    {
        Task<IEnumerable<Assinatura>> GetAssinaturas(int idEstabelecimento);
        Task<Assinatura> GetAssinatura(int id);
        Task<Assinatura> GetAssinatura(string chave_pagamento);
        Task<Assinatura> GetUltimaAssinatura(int idEstabelecimento);
        Task<Assinatura> GetAssinaturaPendente(int idEstabelecimento);
        object AddAssinatura(Assinatura assinatura);
        object UpdateAssinatura(Assinatura assinatura);
        object DeleteAssinatura(int id);

    }
}
