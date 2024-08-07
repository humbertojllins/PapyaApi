using Microsoft.Extensions.Configuration;
using Dapper;
using papya_api.Models;
using Microsoft.AspNetCore.Mvc;
using papya_api.Services;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;

namespace papya_api.DataProvider
{
    public class UsuarioDAO : Controller
    {
        private readonly IConfiguration _configuration;
        Global util = new Global();

        public UsuarioDAO(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Usuario Find(string telefone, string senha)
        {
            string senhaCript = util.CalculateMD5Hash(senha);
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            using (var conexao = new MySqlConnection(strcon))
            {
                Usuario varUsuario =
                 conexao.QueryFirstOrDefault<Usuario>(
                    "select * " +
                    "from usuario " +
                    "where telefone ='" + telefone + "' and " +
                    "senha='" + senhaCript + "'"
                    );
                return varUsuario;

            }
       }
        public void UpdateUsuarioImagem(int IdUsuario, string imagem)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update usuario set " +
                    (imagem == "" ? "" : "imagem ='" + imagem + "' ") +
                    "where id=" + IdUsuario.ToString();
                    conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);

                //UPDATE `usuario` SET `nome` = 'Gerente 1', `senha` = '1234' WHERE `usuario`.`id` = 1
            }
        }

        public Usuario ImagemAtual(int idUsuario)
        {
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            using (var conexao = new MySqlConnection(strcon))
            {
                Usuario varUsuario =
                 conexao.QueryFirstOrDefault<Usuario>(
                    "select * " +
                    "from usuario " +
                    "where id =" + idUsuario
                );
                return varUsuario;

            }
        }

        public Usuario ValidaCpf(string cpf)
        {
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            using (var conexao = new MySqlConnection(strcon))
            {
                Usuario varUsuario =
                 conexao.QueryFirstOrDefault<Usuario>(
                    "select * " +
                    "from usuario " +
                    "where cpf ='" + cpf + "'"
                    ) ;
                return varUsuario;

            }
        }
        public Usuario ValidaTelefone(string telefone)
        {
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            using (var conexao = new MySqlConnection(strcon))
            {
                Usuario varUsuario =
                 conexao.QueryFirstOrDefault<Usuario>(
                    "select * " +
                    "from usuario " +
                    "where telefone ='" + telefone + "'"
                    );
                return varUsuario;

            }
        }
        public Usuario ValidaEmail(string email)
        {
            var strcon = ConnectionHelper.GetConnectionString(_configuration);
            using (var conexao = new MySqlConnection(strcon))
            {
                Usuario varUsuario =
                 conexao.QueryFirstOrDefault<Usuario>(
                    "select * " +
                    "from usuario " +
                    "where email ='" + email + "'"
                    );
                return varUsuario;

            }
        }
    }
}
