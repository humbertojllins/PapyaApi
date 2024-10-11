using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using papya_api.DataProvider;
using papya_api.Models;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController:Controller
    {
        public IChatDataProvider ChatDataProvider;

        public ChatController(IChatDataProvider chatDataProvider)
        {
            this.ChatDataProvider = chatDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<Chat> Get(int usuariofrom, int usuarioTo)
        {
            return await this.ChatDataProvider.GetChat(usuariofrom, usuarioTo);
        }

        [HttpPost]
        public object Post([FromBody] Chat chat)
        {
            return this.ChatDataProvider.AddChat(chat);
        }

        [HttpPut]
        public object Put([FromBody] Chat chat)
        {
            return this.ChatDataProvider.AddMensagem(chat);
        }
        //[Authorize("Bearer")]
        [HttpDelete]
        public object Delete(int idChat, string idMensagem)
        {
            return this.ChatDataProvider.DeleteMensagem(idChat, idMensagem);
        }
    }
}
