using System;
using Microsoft.AspNetCore.Mvc;
using papya_api.DataProvider;
using papya_api.Models;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoItemController : Controller
    {
        [HttpPost]
        public object Post(
            [FromBody] PedidoItem pedidoItem,
            [FromServices] DataProvider.PedidoItemDAO pedidoitemDAO,
            [FromServices] SigningConfigurations signingConfigurations,
            [FromServices] TokenConfigurations tokenConfigurations)
        {
            return pedidoitemDAO.UpdatestatusItem(pedidoItem.ID, pedidoItem.FK_STATUS_ID);
        }
    }
}
