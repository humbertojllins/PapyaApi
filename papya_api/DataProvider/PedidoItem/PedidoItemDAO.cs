using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;
using System.Threading.Tasks;

namespace papya_api.DataProvider
{
    public class PedidoItemDAO : Controller
    {
        private readonly IConfiguration _configuration;


        public PedidoItemDAO(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object UpdatestatusItem(int idPedidoItem, int id_status)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update pedido_itens set " +
                    "fk_status_id =" + id_status + " " +
                    "where id=" + idPedidoItem.ToString();
                ret = conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);
                return ret;
            }
        }
        
    }
}
