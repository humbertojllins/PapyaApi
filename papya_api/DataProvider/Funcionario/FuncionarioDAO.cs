using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using papya_api.Models;
using Microsoft.AspNetCore.Mvc;
using papya_api.ExtensionMethods;

namespace papya_api.DataProvider
{
    public class FuncionarioDAO : Controller
    {
        private readonly IConfiguration _configuration;


        public FuncionarioDAO(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Funcionario> Find(int id)
        {
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            string sql = "select id, id_usuario, f.id_estabelecimento, salario, id_tipofuncionario, e.nome as nomeestabelecimento, e.imagem as imagemestabelecimento  " +
                             "from funcionario f " +
                             "inner join estabelecimento e on f.id_estabelecimento = e.id_estabelecimento " +
                             "where id_usuario =" + id.ToString();
            using (var conexao = new MySqlConnection(strcon))
            {
                return 
                 conexao.QueryAsync<Funcionario>(sql).Result;
            }
       }
        
    }
}
