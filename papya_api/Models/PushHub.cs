using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace papya_api.Models
{
    public class PushHub : Hub
    {
        //[EnableCors("PolicySignalr")]
        //[DisableCors]
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}


        //public static ConcurrentDictionary<string, List<string>> ConnectedUsers;

        public PushHub()
        {
            //ConnectedUsers = new ConcurrentDictionary<string, List<string>>();
        }

        public override Task OnConnectedAsync()
        {
            var userToken = Context.GetHttpContext().Request.Query["access_token"];
            Groups.AddToGroupAsync(Context.ConnectionId, userToken);
            

            //string userid = Context.User.Identity.Name;
            //if (userid == null || userid.Equals(string.Empty))
            //{
            //    Trace.TraceInformation("user not loged in, can't connect signalr service");
            //    return;
            //}


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


        public async Task ListConnectedUsers()
        {
            //List<string> data = ConnectedUsers.Keys;
            await Clients.All.SendAsync("ListConnectedUsers", "sasasas");
        }



    }
}
