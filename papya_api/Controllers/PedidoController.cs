using System;
using System.Collections.Generic;
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
        public IMesaDataProvider MesaDataProvider;
        public IUsuarioDataProvider UsuarioDataProvider;
        private readonly IHubContext<PushHub> _hubContext;

        public PedidoController(IPedidoDataProvider pedidoDataProvider, IHubContext<PushHub> hubcontext, IMesaDataProvider mesaDataProvider, IUsuarioDataProvider usuarioDataprovider)
        {
            this.PedidoDataProvider = pedidoDataProvider;
            this.MesaDataProvider = mesaDataProvider;
            this.UsuarioDataProvider = usuarioDataprovider;
            _hubContext = hubcontext;
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
            string nomeusuario="", mesa = "";
            StringBuilder itens = new StringBuilder();
            //string mensagem = "Novo pedido";
            foreach (var item in a)
            {
                mesa = item.MESA;
                nomeusuario = item.NOME;
                itens.Append(string.Format(" {0}-{1}, " , item.QTD_ITEM , item.ITEM_TITULO)).ToString();
            }
            //var data = (IDictionary<string, object>)a;
            //data["usuario"]
            //data["mesa"]
            //data["estabelecimento"]
            string mensagem = nomeusuario + " da mesa " + mesa + ", solicitou : " + itens;
            this._hubContext.Clients.All.SendAsync("ReceiveMessage",nomeusuario , mensagem);
            return a;
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost("PostPedidoSignalR")]
        public object PostPedidoSignalR(string mesa, string mensagem)
        {
            var a = "Sucesso";
            //this._hubContext.Clients.All.SendAsync("ReceiveMessage", mesa,mensagem);
            //this._hubContext.Clients.Groups().SendAsync("ReceiveMessage", mesa, mensagem);
            this._hubContext.Clients.Group(mesa).SendAsync("ReceiveMessage", mesa, mensagem);
            //this._hubContext.Clients.Users("").SendAsync("ReceiveMessage", mesa, mensagem);
            //this._hubContext.Clients.User("").SendAsync("ReceiveMessage", mesa, mensagem);

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
