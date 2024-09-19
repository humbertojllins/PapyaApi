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
    public class MesaController : Controller
    {
        public IMesaDataProvider MesaDataProvider;

        public MesaController(IMesaDataProvider mesaDataProvider)
        {
            this.MesaDataProvider = mesaDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<Mesa>> Get(int idEstabelecimento)
        {
            return await this.MesaDataProvider.GetMesas(idEstabelecimento);
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<Mesa> Get1(int id)
        {
            return await this.MesaDataProvider.GetMesa(id);
        }

        // POST api/values
        //[Authorize("Bearer")]
        [HttpPost]
        public object Post([FromBody] Mesa mesa)
        {
            return this.MesaDataProvider.AddMesa(mesa);
        }

        // PUT api/values/5
        //[Authorize("Bearer")]
        [HttpPut]
        public object Put([FromBody] Mesa mesa)
        {
            return this.MesaDataProvider.UpdateMesa(mesa);
        }

        // DELETE api/values/5
        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            return this.MesaDataProvider.DeleteMesa(id);
        }

    }
}