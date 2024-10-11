using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using papya_api.DataProvider;
using System.Linq;
using papya_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using papya_api.Services;
using Microsoft.AspNetCore.Hosting;

namespace papya_api.Models
{
    //[Authorize(Policy = "MyPolicy")]
    public class PushHub : Hub
    {
        private readonly IConfiguration _configuration;
        public IChatDataProvider _chatDataProvider;
        public IUsuarioDataProvider _usuarioDataProvider;
        public static IHostingEnvironment _environment;

        //ChatController _chatController;
        private Chat chat;
        Global g = new Global();

        //[EnableCors("PolicySignalr")]
        //[DisableCors]
        public async Task SendMessage(string user, string message, string autorizado="1")
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, autorizado);
        }

        
        public async Task SendMessageToUser(string userFrom, string userTo, string message, string idChat ="0")
        {
            try
            {
                chat = new Chat();
                chat.id = Convert.ToInt32(idChat);
                chat.usuariofrom = Convert.ToInt32(userFrom);
                chat.usuarioto = Convert.ToInt32(userTo);
                chat.mensagem = _chatDataProvider.Mensagem(chat.usuariofrom, chat.usuarioto, message);

                string autorizacao = idChat == "0" ? "0" : "1";

                //Enviar a mensagem pro SignalR
                await Clients.Group(userTo).SendAsync("ReceiveMessage", userFrom, chat.mensagem, autorizacao);
                await Clients.Group(userFrom).SendAsync("ReceiveMessage", userFrom, chat.mensagem, autorizacao);
                //Enviar a mensagem pro SignalR

                //Grava a mensagem no banco
                if (chat.id == 0)
                {   
                    _chatDataProvider.AddChat(chat);
                }
                else
                {
                    _chatDataProvider.AddMensagem(chat);
                }
                string pathProjeto = _environment.ContentRootPath;
                //Criar rotina para buscar a chave do usuário no banco
                Usuario uTo = _usuarioDataProvider.GetUsuario(Convert.ToInt32(userTo)).Result;
                Usuario uFrom = _usuarioDataProvider.GetUsuario(Convert.ToInt32(userFrom)).Result;

                //Envia a notificação ao dispositivo cliente
                g.EnviarNotificacaoFirebase(pathProjeto,uTo.chaveNotificacao,uFrom.Nome, message,"https://images.papya.com.br" + uFrom.Imagem,userFrom, userTo);
                

                //await Clients.User(userTo).SendAsync("ReceiveMessage", userFrom, message);
                //await Clients.User(userFrom).SendAsync("ReceiveMessage", userFrom, message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public static ConcurrentDictionary<string, List<string>> ConnectedUsers;

        //public PushHub()
        //{

        //}
        public PushHub(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            _chatDataProvider = new ChatDataProvider(configuration);
            _usuarioDataProvider = new UsuarioDataProvider(configuration, null,environment);
        }
        public override Task OnConnectedAsync()
        {   
            var userToken = Context.GetHttpContext().Request.Query["access_token"];
            Groups.AddToGroupAsync(Context.ConnectionId, userToken);

            

            //Trace.TraceInformation(userid + "connected");
            //// save connection
            //List<string> existUserConnectionIds;
            //ConnectedUsers.TryGetValue(userid, out existUserConnectionIds);
            //if (existUserConnectionIds == null)
            //{
            //    existUserConnectionIds = new List<string>();
            //}
            //existUserConnectionIds.Add(Context.ConnectionId);
            //ConnectedUsers.TryAdd(userid, existUserConnectionIds);

            //await Clients.All.SendAsync("ServerInfo", userid, userid + " connected, connectionId = " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }


        public async Task ListConnectedUsers(IConfiguration configuration)
        {
            //List<string> data = ConnectedUsers.Keys;
            await Clients.All.SendAsync("ListConnectedUsers", "sasasas");
        }
    }
}
