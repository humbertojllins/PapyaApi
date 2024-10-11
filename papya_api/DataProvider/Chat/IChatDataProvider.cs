using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;


namespace papya_api.DataProvider
{
    public interface IChatDataProvider
    {
        Task<Chat> GetChat(int usuarioFrom, int usuarioTo);
        Object AddChat(Chat chat);
        Object AddMensagem(Chat chat);
        Object DeleteMensagem(int idChat, string idMensagem);

        string Mensagem(int from, int to, string mensagem);
    }
}
