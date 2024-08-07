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
    public class StatusItemController : Controller
    {
        public IStatusItemDataProvider StatusItemDataProvider;

        public StatusItemController(IStatusItemDataProvider statusItemDataProvider)
        {
            this.StatusItemDataProvider = statusItemDataProvider;
        }

        //[Authorize("Bearer")]
        [HttpGet]
        public async Task<IEnumerable<StatusItem>> Get()
        {
            return await this.StatusItemDataProvider.GetStatusItems();
        }

        // GET api/values/5
        //[Authorize("Bearer")]
        [HttpGet("{id}")]
        public async Task<StatusItem> Get(int id)
        {
            return await this.StatusItemDataProvider.GetStatusItem(id);
        }

        // POST api/values
        [Authorize("Bearer")]
        [HttpPost]
        public async Task Post([FromBody] StatusItem StatusItem)
        {
            await this.StatusItemDataProvider.AddStatusItem(StatusItem);
        }

        // PUT api/values/5
        [Authorize("Bearer")]
        [HttpPut]
        public async Task Put(int id, [FromBody] StatusItem StatusItem)
        {
            await this.StatusItemDataProvider.UpdateStatusItem(StatusItem);
        }

        // DELETE api/values/5
        [Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.StatusItemDataProvider.DeleteStatusItem(id);
        }

    }
}
