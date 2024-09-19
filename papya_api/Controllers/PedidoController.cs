using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using papya_api.DataProvider;
using papya_api.Models;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController:Controller
    {
        public IPedidoDataProvider PedidoDataProvider;
        public IAssinaturaDataProvider MesaDataProvider;
        public IUsuarioDataProvider UsuarioDataProvider;
        private readonly IHubContext<PushHub> _hubContext;
        private INotificacaoDataProvider _notificacaoDataProvider;

        public PedidoController(IPedidoDataProvider pedidoDataProvider, IHubContext<PushHub> hubcontext, IAssinaturaDataProvider mesaDataProvider, IUsuarioDataProvider usuarioDataprovider, INotificacaoDataProvider notificacaoDataProvider)
        {
            this.PedidoDataProvider = pedidoDataProvider;
            this.MesaDataProvider = mesaDataProvider;
            this.UsuarioDataProvider = usuarioDataprovider;
            _hubContext = hubcontext;
            _notificacaoDataProvider = notificacaoDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public object Get(int id)
        {
            return this.PedidoDataProvider.GetPedidos(id);
        }


        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public object Post([FromBody] IEnumerable<Pedido> lista)
        {
            var a = this.PedidoDataProvider.AddPedido(lista) as IEnumerable<UltimoPedido>;
            int idestabelecimento = 0;
            string nomeusuario="", mesa = "";
            StringBuilder itens = new StringBuilder();
            //string mensagem = "Novo pedido";
            foreach (var item in a)
            {
                idestabelecimento = item.idestabelecimento;
                mesa = item.mesa;
                nomeusuario = item.nome;
                itens.Append(string.Format(" {0}-{1}, " , item.qtd_item, item.item_titulo)).ToString();
            }
            itens.ToString().Remove(itens.ToString().LastIndexOf(","), 1);
            //var payload = new JavaScriptSerializer().Serialize(new
            //{
            //    notification = new
            //    {
            //        title = obj.payload.title,
            //        body = obj.payload.body,
            //        icon = obj.payload.icon,
            //        vibrate = obj.payload.vibrate,
            //    }
            //});
            string mensagem = nomeusuario + " da mesa " + mesa + ", solicitou : " + itens.ToString().Remove(itens.ToString().LastIndexOf(","), 1);
            _notificacaoDataProvider.Notify(idestabelecimento, mensagem);
            


            //this._hubContext.Clients.All.SendAsync("ReceiveMessage",nomeusuario , mensagem);
            return a;
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost("PostPedidoSignalR")]
        public object PostPedidoSignalR(string mesa, string mensagem)
        {
            var a = "Sucesso";
            this._hubContext.Clients.Group(mesa).SendAsync("ReceiveMessage", mesa, mensagem);
            return a;
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        //[HttpPut("{id}")]
        //[HttpPut]
        //public async Task Put(int item)
        //{
        //await this.CardapioDataProvider.UpdateCardapio(usuario);
        //}

        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.PedidoDataProvider.DeletePedido(id);
        }
    }
}
