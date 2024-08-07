using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using papya_api.DataProvider;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContaDetalheController:Controller
    {
        public IContaDetalheDataProvider ContaDetalheDataProvider;

        public ContaDetalheController(IContaDetalheDataProvider contaDetalheDataProvider)
        {
            this.ContaDetalheDataProvider = contaDetalheDataProvider;
        }

        //[Authorize("Bearer")]
        //[HttpGet("{id}")]
        [HttpGet]
        public object Get(int id)
        {
            return this.ContaDetalheDataProvider.GetDetalheContas(id);
        }

        [HttpGet("GetByEstabelecimento")]
        public object GetByEstabelecimento(int idEstabelecimento, int? idFuncionario, int? is_cozinha, int? status_conta, int? status_item)
        {
            return this.ContaDetalheDataProvider.GetDetalheContasNovo(idEstabelecimento, idFuncionario,is_cozinha, status_conta, status_item);
        }

    }
}
