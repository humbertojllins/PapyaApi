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
    public class CardapioController:Controller
    {
        public ICardapioDataProvider CardapioDataProvider;

        public CardapioController(ICardapioDataProvider CardapioDataProvider)
        {
            this.CardapioDataProvider = CardapioDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public object Get(int id)
        {
            return this.CardapioDataProvider.GetCardapios(id);
        }

        //Alteração teste GITHUB
        // POST api/values
        //[Authorize("Bearer")]
        //[HttpPost]
        //public object Post(Cardapio cardapio, IFormFile foto,
        //    [FromServices] DataProvider.CardapioDAO cardapioDAO)
        //{
        //    object retorno = this.CardapioDataProvider.AddCardapio(cardapio.ITEM_TITULO, cardapio.ITEM_DESCRICAO,cardapio.VALOR_ITEM, cardapio.TEMPO_ESTIMADO_MIN, cardapio.TEMPO_ESTIMADO_MAX,cardapio.IMAGEM,cardapio.ITEM_ID_CATEGORIA, cardapio.FK_ID_ESTABELECIMENTO,cardapio.IS_COZINHA,cardapio.IS_CARDAPIO);
        //    cardapioDAO.Upload(Convert.ToInt32(retorno), foto);
        //    return new { retorno = "Cadastro realizado com sucesso." };
        //}

        [HttpPost]
        public object Post(
            [FromForm] string titulo,
            [FromForm] string descricao,
            [FromForm] double valor,
            [FromForm] int tempo_estimado_min,
            [FromForm] int tempo_estimado_max,
            [FromForm] int categoria,
            [FromForm] int estabelecimento,
            [FromForm] int iscozinha,
            [FromForm] int iscardapio,
            [FromForm] IFormFile files,
            [FromServices] DataProvider.CardapioDAO cardapioDAO)
        {
            object retorno = this.CardapioDataProvider.AddCardapio(titulo,descricao,valor,tempo_estimado_min,tempo_estimado_max,categoria,estabelecimento,iscozinha,iscardapio);
            return cardapioDAO.Upload(Convert.ToInt32(retorno), files);

            //return new { retorno = "Cadastro realizado com sucesso." };
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        //[HttpPut("{id}")]
        [HttpPut]
        public object Put(
            [FromForm] int id,
            [FromForm] string titulo,
            [FromForm] string descricao,
            [FromForm] double valor,
            [FromForm] int tempo_estimado_min,
            [FromForm] int tempo_estimado_max,
            [FromForm] int categoria,
            [FromForm] int estabelecimento,
            [FromForm] int iscozinha,
            [FromForm] int iscardapio,
            [FromForm] IFormFile files,
            [FromServices] DataProvider.CardapioDAO cardapioDAO)
        {
            this.CardapioDataProvider.UpdateCardapio(id,titulo, descricao, valor, tempo_estimado_min, tempo_estimado_max, categoria, estabelecimento, iscozinha, iscardapio);
            if (files != null) { 
                return cardapioDAO.Upload(id, files);
            }
            
            return StatusCode(200, new { Retorno = "Dados alterados" });
        }

        [HttpPut("PutSemArquivo")]
        public object PutSemArquivo(
            [FromForm] int id,
            [FromForm] string titulo,
            [FromForm] string descricao,
            [FromForm] double valor,
            [FromForm] int tempo_estimado_min,
            [FromForm] int tempo_estimado_max,
            [FromForm] int categoria,
            [FromForm] int estabelecimento,
            [FromForm] int iscozinha,
            [FromForm] int iscardapio,
            [FromServices] DataProvider.CardapioDAO cardapioDAO)
        {
            this.CardapioDataProvider.UpdateCardapio(id, titulo, descricao, valor, tempo_estimado_min, tempo_estimado_max, categoria, estabelecimento, iscozinha, iscardapio);
           
            return StatusCode(200, new { Retorno = "Dados alterados" });
        }
        


        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            return this.CardapioDataProvider.DeleteCardapio(id);
        }
    }
}
