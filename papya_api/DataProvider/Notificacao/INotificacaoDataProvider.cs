﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface INotificacaoDataProvider
    {
        Task<IEnumerable<Notificacao>> GetNotificacaos(int idEstabelecimento, string client = "");

        Task<Notificacao> GetNotificacao(string client);

        object AddNotificacao(Notificacao Notificacao);

        object UpdateNotificacao(Notificacao Notificacao);

        object Notify(int idEstabelecimento, string message, string? client = null);

        Task<object> NotificacaoCadastrada(int idEstabelecimento, string client);

        //Task UpdateNotificacao(Notificacao Notificacao);

        //Task DeleteNotificacao(int CodNotificacao);

    }
}
