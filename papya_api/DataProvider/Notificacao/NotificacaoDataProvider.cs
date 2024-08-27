using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using WebPush;
using System.Linq;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;

namespace papya_api.DataProvider
{
    public class NotificacaoDataProvider : INotificacaoDataProvider
    {

        private readonly IConfiguration _configuration;

        public NotificacaoDataProvider()
        {

        }

        public NotificacaoDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object AddNotificacao(Notificacao Notificacao)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                try
                {
                    conexao.Open();
                    var sql = "insert into notificacao (fkestabelecimento, client,endpoint,p256dh,auth)" +
                        " values(" +
                    "" + Notificacao.Fkestabelecimento + "," +
                    "'" + Notificacao.Client + "'," +
                    "'" + Notificacao.EndPoint + "'," +
                    "'" + Notificacao.P256dh + "'," +
                    "'" + Notificacao.Auth + "');";

                    return conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public object UpdateNotificacao(Notificacao Notificacao)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                try
                {
                    conexao.Open();
                    var sql = "UPDATE notificacao" +
                                " SET " +
                                " fkestabelecimento =" + Notificacao.Fkestabelecimento + "," +
                                " client =" + "'" + Notificacao.Client + "'," +
                                " endpoint=" + "'" + Notificacao.EndPoint + "'," +
                                " p256dh =" + "'" + Notificacao.P256dh + "'," +
                                " auth=" + "'" + Notificacao.Auth + "'" +
                                " WHERE fkestabelecimento = " + Notificacao.Fkestabelecimento +
                                " AND client =" + "'" + Notificacao.Client + "'";

                    return conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        

        public async Task<Notificacao> GetNotificacao(string client)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Notificacao>(
                    "select * from notificacao where client='" + client + "'",
                    null,
                    commandType: System.Data.CommandType.Text);
            }
        }
        public async Task<IEnumerable<Notificacao>> GetNotificacaos(int idEstabelecimento, string client="")
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select * ";
                sql+= "from notificacao  ";
                sql += " where fkestabelecimento=" + idEstabelecimento;
                if(client!=null && client!="")
                    sql += " and client='" + client + "'";
                conexao.Open();
                return await conexao.QueryAsync<Notificacao>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<object> NotificacaoCadastrada(int idEstabelecimento, string client)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select count(1) existe ";
                sql += "from notificacao  ";
                sql += " where fkestabelecimento=" + idEstabelecimento;
                sql += " and client='" + client+ "'";
                conexao.Open();
                var ret = conexao.QuerySingleOrDefault(
                sql,
                null,
                commandType: System.Data.CommandType.Text);

                return ret;
            }
        }

        public object Notify(int idEstabelecimento, string client, string message)
        {
            var subject = _configuration["VAPID:subject"];
            var publicKey = _configuration["VAPID:publicKey"];
            var privateKey = _configuration["VAPID:privateKey"];

            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);


            List<Notificacao> lista = GetNotificacaos(idEstabelecimento, client).Result.Cast<Notificacao>().ToList();
            //List<DadosUsuario> l = GetListaUsuarios().Result.Cast<DadosUsuario>().ToList();
            PushSubscription subscription;
            var webPushClient = new WebPushClient();
            foreach (var item in lista)
            {
                subscription = new PushSubscription() { Auth = item.Auth, Endpoint = item.EndPoint, P256DH = item.P256dh };

                try
                {
                    webPushClient.SendNotification(subscription, message, vapidDetails);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }

            return "OK";
        }

    }
}
