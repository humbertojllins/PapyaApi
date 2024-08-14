using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using papya_api.Models;
using papya_api.DataProvider;
using Microsoft.AspNetCore.Authorization;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacaoController : Controller
    {
        public INotificacaoDataProvider NotificacaoDataProvider;

        public NotificacaoController(INotificacaoDataProvider NotificacaoDataProvider)
        {
            this.NotificacaoDataProvider = NotificacaoDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<Notificacao>> Get(int idEstabelecimento)
        {
            return await this.NotificacaoDataProvider.GetNotificacaos(idEstabelecimento);
        }
        [HttpGet("{client}")]
        public async Task<Notificacao> Get(string client)
        {
            return await this.NotificacaoDataProvider.GetNotificacao(client);
        }

        [HttpGet("{idEstabelecimento}/{client}")]
        public async Task<object> Get(int idEstabelecimento, string client)
        {
            return await this.NotificacaoDataProvider.NotificacaoCadastrada(idEstabelecimento, client);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public object Post([FromBody] Notificacao Notificacao)
        {
            return this.NotificacaoDataProvider.AddNotificacao(Notificacao);
        }

        [HttpPut]
        public object Put([FromBody] Notificacao Notificacao)
        {
            return this.NotificacaoDataProvider.UpdateNotificacao(Notificacao);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost("Notify")]
        public object Notify(int idEstabelecimento, string message)
        {
            return this.NotificacaoDataProvider.Notify(idEstabelecimento, message);
        }

    }
}
