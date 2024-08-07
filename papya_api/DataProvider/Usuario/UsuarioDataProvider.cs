using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using papya_api.ExtensionMethods;
using papya_api.Models;
using papya_api.Services;

namespace papya_api.DataProvider
{
    public class UsuarioDataProvider : IUsuarioDataProvider
    {

        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        Global util = new Global();

        public UsuarioDataProvider()
        {
            
        }

        public UsuarioDataProvider(IConfiguration configuration, IEmailSender emailSender, IHostingEnvironment env)
        {
            _configuration = configuration;
            _emailSender = emailSender;
        }

        //public UsuarioDataProvider(IEmailSender emailSender, IHostingEnvironment env)
        //{
        //    _emailSender = emailSender;
        //}

        public object AddUsuario(Usuario usuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string senhaCript = util.CalculateMD5Hash(usuario.Senha);

                conexao.Open();
                var sql = "insert into usuario (id_tipo_usuario, nome, cpf, nascimento, senha,email,resetar_Senha,imagem, telefone)" +
                    " values( " +
                    "2," +
                    "'" + usuario.Nome + "'," +
                    "'" + usuario.Cpf + "'," +
                    "'" + usuario.Nascimento + "'," +
                    "'" + senhaCript + "'," +
                    "'" + usuario.Email + "'," +
                    "0," +
                    "'" + usuario.Imagem + "'," +
                    "'" + usuario.Telefone + "')";
                var ret = conexao.Execute(sql, commandType: System.Data.CommandType.Text);
                return ret;
            }
        }

        public Task DeleteUsuario(int CodTipoUsuario)
        {
            throw new NotImplementedException();
        }

        public async Task<Usuario> GetUsuario(int CodUsuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Usuario>(
                    "select * from usuario where id=" + CodUsuario.ToString(),
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        public object GetUsuarioMesa(int idUsuarioConta)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select distinct u.nome as usuario, m.descricao as mesa, m.fk_id_estabelecimento as estabelecimento " +
                            " from pedido p" +
                            " left join usuario_conta uc on uc.id = p.fk_usuario_conta_id" +
                            " left join usuario u on u.id = uc.id_usuario" +
                            " left join mesa m on p.fk_id_mesa = m.id" +
                            " where fk_usuario_conta_id =" + idUsuarioConta;

                conexao.Open();
                return conexao.QuerySingleOrDefault(sql,
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }

        



        public async Task<Usuario> GetUsuarioCpf(string cpf)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Usuario>(
                    "select * from usuario where cpf='" + cpf + "'",
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }


        public async Task<IEnumerable<Usuario>> GetUsuarios()
        {
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

        //public async Task<IEnumerable<UsuarioSocial>> GetUsuariosSocial(float latitude, float longitude, float distanciaKm, int? qtdLista, int? idEstabelecimento)
        //{
        //    using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
        //    //using (var conexao = new MySqlConnection(strConexao))
        //    {
        //        string sql = "SELECT ";
        //        sql += qtdLista == null ? " " : " top " + qtdLista;
        //        sql += " U.ID,";
        //        sql += "U.ID_TIPO_USUARIO, ";
        //        sql += "U.NOME, ";
        //        sql += "U.SOBRENOME, ";
        //        sql += "U.CPF, ";
        //        sql += "U.NASCIMENTO, ";
        //        sql += "U.STATUSSOCIAL, ";
        //        sql += "U.EMAIL, ";
        //        sql += "U.IMAGEM, ";
        //        sql += "U.TELEFONE, ";
        //        sql += "E.NOME AS ESTABELECIMENTONOME,";
        //        sql += "E.ENDERECO AS ESTABELECIMENTOENDERECO,";
        //        sql += "E.LATITUDE AS ESTABELECIMENTOLATITUDE,";
        //        sql += "E.LONGITUDE AS ESTABELECIMENTOLONGITUDE,";
        //        sql += "E.IMAGEM AS ESTABELECIMENTOIMAGEM, ";
        //        sql += "(" +
        //                "acos(sin(radians(latitude))" +
        //                "* sin(radians(" + latitude + "))" +
        //                "+ cos(radians(latitude))" +
        //                "* cos(radians(" + latitude + "))" +
        //                "* cos(radians(longitude) " +
        //                "- radians(" + longitude + " ))) * 6378 " +
        //                ") distanciakm ";

        //        sql += "FROM USUARIO U ";
        //        sql += "INNER JOIN USUARIO_CONTA UC ON U.ID = UC.ID_USUARIO AND UC.PUBLICO = 1 ";
        //        sql += "INNER JOIN CONTA C ON C.ID = UC.ID_CONTA AND C.ID_STATUS = 1 ";
        //        sql += "INNER JOIN MESA M ON M.ID = C.ID_MESA ";
        //        sql += "INNER JOIN ESTABELECIMENTO E ON M.fk_id_estabelecimento = E.ID_ESTABELECIMENTO ";

        //        sql += "where (" +
        //                "acos(sin(radians(latitude))" +
        //                "* sin(radians(" + latitude + "))" +
        //                "+ cos(radians(latitude))" +
        //                "* cos(radians(" + latitude + "))" +
        //                "* cos(radians(longitude) " +
        //                "- radians(" + longitude + " ))) * 6378 " +
        //                ") <=" + distanciaKm;
        //        sql += (idEstabelecimento == null ? "" : " AND M.FK_ID_ESTABELECIMENTO = " + idEstabelecimento);

        //        try
        //        {
        //            conexao.Open();
        //            return await conexao.QueryAsync<UsuarioSocial>(
        //            sql,
        //            null,
        //            commandType: System.Data.CommandType.Text);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //}

        public async Task<IEnumerable<UsuarioSocial>> GetUsuariosSocial(float latitude, float longitude, float distanciaKm, int? qtdLista, int? idEstabelecimento)
        {
            using (var conexao = new MySqlConnection(_configuration.GetConnectionString("tingledb")))
            //using (var conexao = new MySqlConnection(strConexao))
            {
                string sql = "SELECT ";
                sql += "U.ID,";
                sql += "U.ID_TIPO_USUARIO, ";
                sql += "U.NOME, ";
                sql += "U.SOBRENOME, ";
                sql += "U.CPF, ";
                sql += "U.NASCIMENTO, ";
                sql += "U.STATUSSOCIAL, ";
                sql += "U.EMAIL, ";
                sql += "U.IMAGEM, ";
                sql += "U.TELEFONE, ";
                sql += "E.NOME AS ESTABELECIMENTONOME,";
                sql += "E.ENDERECO AS ESTABELECIMENTOENDERECO,";
                sql += "E.LATITUDE AS ESTABELECIMENTOLATITUDE,";
                sql += "E.LONGITUDE AS ESTABELECIMENTOLONGITUDE,";
                sql += "E.IMAGEM AS ESTABELECIMENTOIMAGEM, ";
                sql += "(" +
                        "acos(sin(radians(latitude))" +
                        "* sin(radians(" + latitude + "))" +
                        "+ cos(radians(latitude))" +
                        "* cos(radians(" + latitude + "))" +
                        "* cos(radians(longitude) " +
                        "- radians(" + longitude + " ))) * 6378 " +
                        ") distanciakm ";

                sql += "from tingledb.usuario u ";
                sql += "inner join tingledb.usuario_conta uc on u.id = uc.id_usuario and uc.publico = 1 ";
                sql += "inner join tingledb.conta c on c.id = uc.id_conta and c.id_status = 1 ";
                sql += "inner join tingledb.mesa m on m.id = c.id_mesa ";
                sql += "inner join tingledb.estabelecimento e on m.fk_id_estabelecimento = e.id_estabelecimento ";

                sql += "where (" +
                        "acos(sin(radians(latitude))" +
                        "* sin(radians(" + latitude + "))" +
                        "+ cos(radians(latitude))" +
                        "* cos(radians(" + latitude + "))" +
                        "* cos(radians(longitude) " +
                        "- radians(" + longitude + " ))) * 6378 " +
                        ") <=" + distanciaKm;
                sql += (idEstabelecimento == null ? "" : " AND M.FK_ID_ESTABELECIMENTO = " + idEstabelecimento);
                sql += qtdLista == null ? " " : " limit " + qtdLista;

                try
                {
                    conexao.Open();
                    return await conexao.QueryAsync<UsuarioSocial>(
                    sql,
                    null,
                    commandType: System.Data.CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<int?> UpdateUsuario(Usuario usuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string senhaCript = util.CalculateMD5Hash(usuario.Senha);

                conexao.Open();
                var sql = "update usuario set " +
                    (usuario.Id_Tipo_Usuario == null ? "" : "id_tipo_usuario ='" + usuario.Id_Tipo_Usuario + "',") +
                    (usuario.Nome==null ? "" : "nome ='" + usuario.Nome + "',") +
                    (usuario.Cpf == null ? "" : "cpf ='" + usuario.Cpf + "',") +
                    (usuario.Nascimento == null ? "" : "nascimento ='" + usuario.Nascimento + "',") +
                    (usuario.Senha == null ? "" : "senha ='" + senhaCript + "',") +
                    (usuario.Email == null ? "" : "email ='" + usuario.Email + "',") +
                    (usuario.Telefone == null ? "" : "telefone ='" + usuario.Telefone + "',") +
                    (usuario.Imagem == null ? "" : "imagem ='" + usuario.Imagem + "', ") +
                    "resetar_Senha =0 " + 
                    " where id=" + usuario.Id;
                int x = await conexao.ExecuteAsync(sql
                    , commandType: System.Data.CommandType.Text);
                return x;

                //UPDATE `usuario` SET `nome` = 'Gerente 1', `senha` = '1234' WHERE `usuario`.`id` = 1
            }
        }
        public object UpdateStatusUsuario(Usuario usuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update usuario set " +
                    (usuario.StatusSocial == null ? "" : "statusSocial ='" + usuario.StatusSocial + "'") +
                    " where id=" + usuario.Id;
                return conexao.ExecuteAsync(sql, commandType: System.Data.CommandType.Text).Result;
                

                //UPDATE `usuario` SET `nome` = 'Gerente 1', `senha` = '1234' WHERE `usuario`.`id` = 1
            }
        }


        public async Task<int?> EsqueceuSenha(string email)
        {
            try
            {
                int? a = null;
                using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
                {
                    string senhaGerada = alfanumericoAleatorio(6);
                    string senhaCript = util.CalculateMD5Hash(senhaGerada);

                    conexao.Open();
                    var sql = "update usuario set " +
                       "senha ='" + senhaCript + "' ," +
                       "resetar_Senha =1 " +
                        "where email='" + email + "';";

                    sql += " select id from usuario ";
                    sql += "where email='" + email + "';";

                    string textBody = "Papya - Transforme seu estabelecimento em uma rede social\r\n"
                                       + "Você solicitou um reset de senha, no próximo login crie uma nova senha "
                                       + "Sua nova senha é :" + senhaGerada;

                    // The HTML body of the email.
                    string htmlBody = @"<html>
                                        <head></head>
                                        <body>
                                          <h2>Você solicitou um reset de senha, no próximo login crie uma nova senha.</h2>
                                          Sua nova senha é : <h4>" + senhaGerada + "</h4>" +
                                        "</body> </html>";

                    a = Convert.ToInt32(conexao.ExecuteScalar(sql
                        , commandType: System.Data.CommandType.Text));

                    if (a != 0)
                    {   
                        await _emailSender.SendEmailAsync(email, "Papya - Troca de senha",htmlBody,textBody);
                    }
                    return a;
                    //return Task.FromResult<object>(null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return Task.FromException(ex);
                //return "";
            }
        }

        public static string alfanumericoAleatorio(int tamanho)
        {
            //var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var chars = "0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
