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
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;

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
            var ret = 0;
            string chaveDinamica = alfanumericoAleatorio(30);
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string senhaCript = util.CalculateMD5Hash(usuario.Senha);
                conexao.Open();
                var sql = "insert into usuario (id_tipo_usuario, nome, cpf, nascimento, senha,email,resetar_Senha,imagem, telefone, chaveNotificacao, emailvalido)" +
                    " values( " +
                    "2," +
                    "'" + usuario.Nome + "'," +
                    "'" + usuario.Cpf + "'," +
                    "'" + usuario.Nascimento + "'," +
                    "'" + senhaCript + "'," +
                    "'" + usuario.Email + "'," +
                    "0," +
                    "'" + usuario.Imagem + "'," +
                    "'" + usuario.Telefone + "'," +
                    "'" + chaveDinamica + "'," +
                    "0" + ")";
                ret = conexao.Execute(sql, commandType: System.Data.CommandType.Text);
            }
            try
            {
                //Configura e envia o e-mail
                string textBody = "Papya - Transforme seu estabelecimento em uma rede social\r\n"
                                       + "Valide seu e-mail para usar o Papya";

                // The HTML body of the email.
                string htmlBody = @"<html>
                                        <head>
                                             <img style=width:200px src=https://images.papya.com.br/Uploads/default.png /> 
											 <h1 style=color:purple;>Transforme seu estabelecimento em uma rede social</h1>
                                        </head>
                                        <body>
                                          <h2>Cadastro realizado com sucesso!</h2>
                                          <h2>Clique <a href=https://api.papya.com.br/api/Usuario/ValidaEmailUsuario?chaveDinamica=" + chaveDinamica + ">aqui </a> e valide seu e-mail para usar o Papya.</h2>" +
                                "</body>" +
                "</html>";

                if (ret != 0)
                {
                    _emailSender.SendEmailAsync(usuario.Email, "Papya - Validação de e-mail", htmlBody, textBody);
                }
            }
            catch (Exception)
            {
                return ret;
            }
            return ret;
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

        public async Task<IEnumerable<UsuarioSocial>> GetUsuariosSocial(float latitude, float longitude, float distanciaKm, int? qtdLista, int? idEstabelecimento, int usuariologado)
        {
            using (var conexao = new MySqlConnection(_configuration.GetConnectionString("tingledb")))
            //using (var conexao = new MySqlConnection(strConexao))
            {
                string sql = "select ";
                sql += "u.id,";
                sql += "u.id_tipo_usuario, ";
                sql += "u.nome, ";
                sql += "u.sobrenome, ";
                sql += "u.cpf, ";
                sql += "u.nascimento, ";
                sql += "u.statussocial, ";
                sql += "u.email, ";
                sql += "u.imagem, ";
                sql += "u.telefone, ";
                sql += "e.nome as estabelecimentonome,";
                sql += "e.endereco as estabelecimentoendereco,";
                sql += "e.latitude as estabelecimentolatitude,";
                sql += "e.longitude as estabelecimentolongitude,";
                sql += "e.imagem as estabelecimentoimagem, ";
                sql += "(" +
                        "acos(sin(radians(latitude))" +
                        "* sin(radians(" + latitude + "))" +
                        "+ cos(radians(latitude))" +
                        "* cos(radians(" + latitude + "))" +
                        "* cos(radians(longitude) " +
                        "- radians(" + longitude + " ))) * 6378 " +
                        ") distanciakm, ";
                //sql += " ifnull(cfrom.dataleiturafrom, cto.dataleiturato) dataleitura, 0 as qtdmensagem ";
                sql += "null dataleitura, 0 as qtdmensagem ";

                sql += "from tingledb.usuario u ";
                sql += "inner join tingledb.usuario_conta uc on u.id = uc.id_usuario and uc.publico = 1 ";
                sql += "inner join tingledb.conta c on c.id = uc.id_conta and c.id_status = 1 ";
                sql += "inner join tingledb.mesa m on m.id = c.id_mesa ";
                sql += "inner join tingledb.estabelecimento e on m.fk_id_estabelecimento = e.id_estabelecimento ";
                //sql += "left join tingledb.chat cfrom on u.id = cfrom.usuariofrom ";
                //sql += "left join tingledb.chat cto on u.id = cto.usuarioto ";
                sql += " where (" +
                        "acos(sin(radians(latitude))" +
                        "* sin(radians(" + latitude + "))" +
                        "+ cos(radians(latitude))" +
                        "* cos(radians(" + latitude + "))" +
                        "* cos(radians(longitude) " +
                        "- radians(" + longitude + " ))) * 6378 " +
                        ") <=" + distanciaKm;
                sql += (idEstabelecimento == null ? "" : " and m.fk_id_estabelecimento = " + idEstabelecimento);
                sql += qtdLista == null ? " " : " limit " + qtdLista;

                try
                {
                    conexao.Open();
                    IEnumerable<UsuarioSocial> lista = await conexao.QueryAsync<UsuarioSocial>(
                    sql,
                    null,
                    commandType: System.Data.CommandType.Text);
                    ChatDataProvider cdp = new ChatDataProvider(_configuration);
                    if (lista != null)
                    {
                        DateTime? dtAtualizacao;
                        //DateTime? dtAtualizacao = lista.Where(c => c.Id == usuariologado).FirstOrDefault().DataLeitura;
                        //dtAtualizacao = dtAtualizacao == null ? Convert.ToDateTime(DateTimeOffset.Now):dtAtualizacao;
                        DateTime dtOrdem = new DateTime(1999, 1, 1);
                        foreach (var item in lista)
                        {
                            Chat c = cdp.GetChatInternal(usuariologado, item.Id).Result;

                            if (c != null)
                            {
                                //Verifica se o usuario que esta logado é o usuario from e retorna a dataatualizacaofrom
                                if (c.usuariofrom == usuariologado)
                                {
                                    dtAtualizacao = c.dataleiturafrom != null ? c.dataleiturafrom : DateTimeOffset.Now.DateTime;
                                }
                                else
                                {
                                    dtAtualizacao = c.dataleiturato != null ? c.dataleiturato : DateTimeOffset.Now.DateTime;
                                }
                                //Recupera a lista de mensagens para remover as mensagens deletadas
                                int qtdMsgFrom = c.listamensagens.Count(m => m.from == usuariologado && m.to == item.Id && m.data > dtAtualizacao);
                                int qtdMsgTo = c.listamensagens.Count(m => m.to == usuariologado && m.from == item.Id && m.data > dtAtualizacao);

                                item.qtdmensagem = qtdMsgFrom + qtdMsgTo;
                                dtOrdem = c.datahora;
                            }
                            item.DataLeitura = dtOrdem;
                        }
                    }
                    return lista.Where(r => r.Id != usuariologado).OrderByDescending(o => o.DataLeitura);
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
                    (usuario.Nome == null ? "" : "nome ='" + usuario.Nome + "',") +
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

        public object UpdateChaveNotificacaoUsuario(Usuario usuario)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update usuario set " +
                    (usuario.chaveNotificacao == null ? "" : "chaveNotificacao ='" + usuario.chaveNotificacao + "'") +
                    " where id=" + usuario.Id;
                return conexao.ExecuteAsync(sql, commandType: System.Data.CommandType.Text).Result;

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
                                        <head>
                                             <img style=""width:200px"" src=""https://images.papya.com.br/Uploads/default.png"" /> 
											 <h1 style=""color:purple;"">Transforme seu estabelecimento em uma rede social</h1>
                                        </head>
                                        <body>
                                          <h2>Você solicitou um reset de senha, no próximo login crie uma nova.</h2>
                                          <h3>Sua senha temporária é : <b style='color:tomato;'>" + senhaGerada + "</b></h3>" +
                                        "</body> "
                                        +
                                        "</html>";

                    a = Convert.ToInt32(conexao.ExecuteScalar(sql
                        , commandType: System.Data.CommandType.Text));

                    if (a != 0)
                    {
                        await _emailSender.SendEmailAsync(email, "Papya - Troca de senha", htmlBody, textBody);
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
        public async Task<string> ValidaEmailUsuario(string chaveDinamica)
        {
            Usuario user;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                try
                {
                    conexao.Open();
                    var sql = "update usuario set emailvalido=1" +
                        " where emailvalido=0 and chaveNotificacao='" + chaveDinamica + "';";
                    await conexao.ExecuteAsync(sql, commandType: System.Data.CommandType.Text);

                    sql = "select * from usuario where chaveNotificacao='" + chaveDinamica + "';";
                    user = await conexao.QuerySingleOrDefaultAsync<Usuario>(sql, null, commandType: System.Data.CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            try
            {
                //Configura o e-mail
                string textBody = "Papya - Transforme seu estabelecimento em uma rede social\r\n"
                                       + "E-mail validado com sucesso ";

                // The HTML body of the email.
                string htmlBody = @"<html>
                                        <head>
                                             <img style=""width:200px"" src=""https://images.papya.com.br/Uploads/default.png"" /> 
											 <h1 style=""color:purple;"">Transforme seu estabelecimento em uma rede social</h1>
                                        </head>
                                        <body>
                                          <h2>E-mail validado com sucesso, aproveite o Papya.</h2>
                                    </body>
                                    </html>";

                if (user != null)
                {
                    await _emailSender.SendEmailAsync(user.Email, "Papya - Validação de e-mail", htmlBody, textBody);
                    return "E-mail validado, aproveite o Papya! ;)";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "Ocorreu algum erro na validação, entre em contato com o suporte do sistema!";
        }
        //public string GeraIdRandom()
        //{
        //    Guid g = Guid.NewGuid();
        //    return Convert.ToBase64String(g.ToByteArray());
        //}
    }
}
