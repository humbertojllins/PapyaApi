using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using papya_api.Models;
using Amazon.SimpleEmail.Model;
using MySqlX.XDevAPI;
using System;

namespace papya_api.DataProvider
{
    public class PedidoItemDAO : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PushHub> _hubContext;


        public PedidoItemDAO(IConfiguration configuration, IHubContext<PushHub> hubcontext)
        {
            _configuration = configuration;
            _hubContext = hubcontext;
        }

        public object UpdatestatusItem(int idPedidoItem, int id_status)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update pedido_itens set " +
                    "fk_status_id =" + id_status + " " +
                    "where id=" + idPedidoItem.ToString() + ";";
                sql +=" select id_usuario from pedido_itens pi";
                sql += " inner join pedido p on pi.id_pedido = p.id";
                sql += " inner join usuario_conta uc on fk_usuario_conta_id = uc.id";
                sql += " where pi.id =" + idPedidoItem.ToString();


                ret = conexao.ExecuteScalar(sql
                    , commandType: System.Data.CommandType.Text);

                //Envia atualização para o aplicativo
                this._hubContext.Clients.Group("CONTA_" + ret.ToString()).SendAsync("ReceiveMessage", "Papya", "Atualizar pedidos");
                return ret;
            }
        }

    }
}
