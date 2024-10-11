using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Collections;
using System.Linq;
using Newtonsoft.Json;

namespace papya_api.DataProvider
{
    public class ChatDataProvider : IChatDataProvider
    {

        private readonly IConfiguration _configuration;
        UsuarioDAO usrDao;

        public ChatDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            usrDao = new UsuarioDAO(configuration);
        }


        public object AddChat(Chat chat)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                //var mensagem = Mensagem(chat.usuariofrom, chat.usuarioto, chat.mensagem);
                var sql = " INSERT INTO chat(datahora,usuariofrom,usuarioto,mensagens)";
                sql += " values(" +
                "'" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "'," +
                "" + chat.usuariofrom + "," +
                "" + chat.usuarioto + "," +
                "CAST('[" + chat.mensagem + "]' as JSON));";
                sql += " select last_insert_id();";
                ret = conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
            }
            return ret;
        }
        public object AddMensagem(Chat chat)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                //update chat set datahora = sysdate() , mensagens = JSON_INSERT(mensagens,'$[9999999]',cast(CONCAT('{"id":"' , @id , '", "data":"', sysdate() ,'","from":2,"to": 73,"mensagem":"OK, mensagem nova"}') as json)) 
                conexao.Open();
                //var mensagem = Mensagem(chat.usuariofrom, chat.usuarioto, chat.mensagem);
                var sql = " UPDATE chat SET usuariotoAutorizacao = 1, datahora=" + "'" + DateTimeOffset.Now.ToString("yyy-MM-dd HH:mm:ss") + "'," +
                    "mensagens = JSON_INSERT(mensagens,'$[9999999]'," + " CAST('" + chat.mensagem + "' AS JSON))" +
                    " WHERE id=" + chat.id;
                ret = conexao.Execute(sql, commandType: System.Data.CommandType.Text);
            }
            return ret;
        }
        public async Task<Chat> GetChat(int usuarioFrom, int usuarioTo)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "SELECT id, datahora, usuariofrom, usuarioto, usuariotoAutorizacao, cast(mensagens as json) mensagens  ";
                sql += "from chat c ";
                sql += " where (usuariofrom =" + usuarioFrom + " and usuarioto =" + usuarioTo;
                sql += ") or (usuariofrom =" + usuarioTo + " and usuarioto =" + usuarioFrom + ")";
                conexao.Open();
                var retorno = await conexao.QuerySingleOrDefaultAsync<Chat>(sql,null,commandType: System.Data.CommandType.Text);
                if (retorno != null)
                {
                    try
                    {
                        //Recupera a lista de mensagens para remover as mensagens deletadas
                        List<Mensagem> lista = JsonConvert.DeserializeObject<List<Mensagem>>(retorno.mensagens);
                        retorno.mensagens = "";
                        //Remove as mensagens deletadas
                        retorno.listamensagens = lista.Where(m => m.id != "0").ToList();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                //Atualiza a data de leitura para controle de notificacão no aplicativo
                try
                {
                    if (retorno.usuariofrom == usuarioFrom)
                    {
                        sql = " update chat set dataleiturafrom='" + DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where usuariofrom=" + usuarioFrom + " and usuarioto =" + usuarioTo + ";";
                    }
                    else
                    {
                        sql = " update chat set dataleiturato='" + DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where usuariofrom=" + usuarioTo + " and usuarioto =" + usuarioFrom + ";";
                    }
                    conexao.Execute(sql, commandType: System.Data.CommandType.Text);
                }
                catch (Exception ex)
                {
                    return retorno;
                }

                return retorno;
            }
        }

        public async Task<Chat> GetChatInternal(int usuarioFrom, int usuarioTo)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "SELECT id, datahora, usuariofrom, usuarioto, usuariotoAutorizacao, dataleiturafrom, dataleiturato, cast(mensagens as json) mensagens  ";
                sql += "from chat c ";
                sql += " where (usuariofrom =" + usuarioFrom + " and usuarioto =" + usuarioTo;
                sql += ") or (usuariofrom =" + usuarioTo + " and usuarioto =" + usuarioFrom + ")";
                conexao.Open();
                var retorno = await conexao.QuerySingleOrDefaultAsync<Chat>(sql, null, commandType: System.Data.CommandType.Text);
                if (retorno != null)
                {
                    try
                    {
                        //Recupera a lista de mensagens para remover as mensagens deletadas
                        List<Mensagem> lista = JsonConvert.DeserializeObject<List<Mensagem>>(retorno.mensagens);
                        retorno.mensagens = "";
                        //Remove as mensagens deletadas
                        retorno.listamensagens = lista.Where(m => m.id != "0").ToList();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return retorno;
            }
        }
        public object DeleteMensagem(int idChat, string idMensagem)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = " UPDATE chat " +
                    "SET mensagens = REPLACE(mensagens," + "'" + '"' + "id" + '"' + ": " + '"' + idMensagem + '"' + "'" + "," + "'" + '"' + "id" + '"' + ": " + '"' + "0" + '"' + "')" +
                    " WHERE id=" + idChat;
                sql = sql.Replace(@"\","");
                ret = conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);
            }
            return ret;
        }
        public string Mensagem(int from, int to, string mensagem)
        {            
            string imgFrom = usrDao.ImagemAtual(from).Imagem;
            string imgTo = usrDao.ImagemAtual(to).Imagem;

            string id = GeraIdRandom();
            string msg = "{";
            msg += '"' + "id" + '"' + ":" + '"' + id + '"' + ",";
            msg += '"' + "data" + '"' + ":" + '"' + DateTimeOffset.Now.ToString("yyy-MM-dd HH:mm:ss") + '"' + ",";
            msg += '"' + "from" + '"' + ":" + from + ",";
            msg += '"' + "to" + '"' + ":" + to + ",";
            msg += '"' + "imgFrom" + '"' + ":" + '"' + imgFrom + '"' + ",";
            msg += '"' + "imgTo" + '"' + ":" + '"' + imgTo + '"' + ",";
            msg += '"' + "mensagem" + '"' + ":" + '"' + mensagem + '"' + "}";

            return msg;
        }
        public string GeraIdRandom()
        {
            Guid g = Guid.NewGuid();
            return Convert.ToBase64String(g.ToByteArray());
        }
    }
}
