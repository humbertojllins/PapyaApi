using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using papya_api.ExtensionMethods;
using papya_api.Models;

namespace papya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        private readonly IConfiguration _configuration;

        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //readonly string strConexao = "Server=191.252.224.218;Port=3306;Database=tingledb;Uid=user_site;Pwd=3A-D4u6C%pRI!\":";
        // GET api/values
        /*[HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            //try
            //{
            //MySqlConnection conn = new MySqlConnection("Server=191.252.224.218;Port=3306;Database=user_site;Uid=user_site;Pwd=3A-D4u6C%pRI!\":");
            //MySqlCommand com = new MySqlCommand("select * from wp_comments", con);
            //conn.Open();
            //MySqlDataAdapter da = new MySqlDataAdapter("select * from wp_comments", conn);
            //System.Data.DataSet ds = new System.Data.DataSet();
            //da.Fill(ds);
            //}
            //catch (MySql.Data.MySqlClient.MySqlException ex)
            //{
            //   throw ex;
            //MessageBox.Show(ex.Message);
            //}
             
           return new string[] { "value1", "value2" };
        }*/

        [HttpGet]
        public async Task<IEnumerable<Usuario>> Get()
        {
            //var strConexaoInicial = new MySqlConnectionStringBuilder(ConnectionHelper.GetConnectionString(_configuration));
            //strConexaoInicial.Password = _configuration["Tingle:Senha"];
            //var strConexao = _configuration.GetConnectionString("tingledb");
            ////var senha = _configuration["Tingle:Senha"];
            //var strConexao = strConexaoInicial.ConnectionString;
            //ConnectionHelper c = new ConnectionHelper(_configuration);
            //var strConexao = ConnectionHelper.GetConnectionString(_configuration);


            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            //using (var conexao = new MySqlConnection(strConexao))
            {
                conexao.Open();

                return await conexao.QueryAsync<Usuario>(
                "select * from usuario",
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
