using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;

namespace papya_api.DataProvider
{
    public class FuncionarioDataProvider : IFuncionarioDataProvider
    {

        private readonly IConfiguration _configuration;

        public FuncionarioDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object AddFuncionario(int idEstabelecimento, int idUsuario, int id_tipo_funcionario)
        {
            object validacao = ValidaCadastroFuncionario(idEstabelecimento, idUsuario).ToString();
            object ret;
            if (validacao.Equals("0"))
            {
                using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
                {
                    conexao.Open();
                    var sql = "insert into funcionario (id_estabelecimento, id_usuario,id_tipofuncionario)" +
                        " values(" +
                        "" + idEstabelecimento + "," +
                        "" + idUsuario + "," +
                        "" + id_tipo_funcionario + ")";
                    ret = conexao.Execute(sql
                        , commandType: System.Data.CommandType.Text);
                    conexao.Close();
                }
                return ret;
            }
            else
            {
                return new
                {
                    retorno = "Retorno:406"
                    
                };
            }
        }

        public object DeleteFuncionario(int idFuncionario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "DELETE FROM funcionario ";
                sql += " WHERE id=" + idFuncionario.ToString();
                conexao.Open();
                return  conexao.Execute(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }            
        }

        public Task<Funcionario> GetFuncionario(int? idFuncionario)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetFuncionarios(int? id_estabelecimento)
        {   
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select  f.id as id_funcionario, u.cpf, u.nome, u.sobrenome, u.telefone, u.imagem,tf.id as id_tipo_funcionario, tf.descricao";
                sql+= " from funcionario f";
                sql+= " inner join usuario u on u.id = f.id_usuario";
                sql+= " inner join tipo_funcionario tf on tf.id = f.id_tipofuncionario ";
                sql += " where f.id_estabelecimento=" + id_estabelecimento.ToString();
                conexao.Open();
                return await conexao.QueryAsync<object>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public object UpdateFuncionario(Funcionario Funcionario)
        {
            throw new NotImplementedException();
        }

        public object ValidaCadastroFuncionario(int? idEstabelecimento, int? idUsuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                string sql = "select count(1)retorno " +
                " from funcionario " +
                " where id_usuario=" + idUsuario +
                " and id_estabelecimento=" + idEstabelecimento + ";";

                return conexao.QuerySingleOrDefault<string>(sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }
    }
}
