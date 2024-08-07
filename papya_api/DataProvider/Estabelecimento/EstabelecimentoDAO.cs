using Microsoft.Extensions.Configuration;
using Dapper;
using papya_api.Models;
using Microsoft.AspNetCore.Mvc;
using papya_api.Services;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;

namespace papya_api.DataProvider
{
    public class EstabelecimentoDAO : Controller
    {
        private readonly IConfiguration _configuration;
        Global util = new Global();

        public EstabelecimentoDAO(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Estabelecimento Find(string cnpj, string senha)
        {
            string senhaCript = util.CalculateMD5Hash(senha);
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            using (var conexao = new MySqlConnection(strcon))
            {
                Estabelecimento varEstabelecimento =
                 conexao.QueryFirstOrDefault<Estabelecimento>(
                    "select * " +
                    "from estabelecimento " +
                    "where cnpj ='" + cnpj + "' and " +
                    "senha='" + senhaCript + "'"
                    );
                return varEstabelecimento;

            }
        }

        public void UpdateEstabelecimentoImagem(int IdEstabelecimento, string imagem)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update estabelecimento set " +
                    (imagem == "" ? "" : "imagem ='" + imagem + "' ") +
                    "where id_estabelecimento=" + IdEstabelecimento.ToString();
                    conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);

                //UPDATE `usuario` SET `nome` = 'Gerente 1', `senha` = '1234' WHERE `usuario`.`id` = 1
            }
        }

        public Estabelecimento ImagemAtual(int idEstabelecimento)
        {
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            using (var conexao = new MySqlConnection(strcon))
            {
                Estabelecimento varEstabelecimento =
                 conexao.QueryFirstOrDefault<Estabelecimento>(
                    "select * " +
                    "from estabelecimento " +
                    "where id_estabelecimento =" + idEstabelecimento
                );
                return varEstabelecimento;

            }
        }
    }
}
