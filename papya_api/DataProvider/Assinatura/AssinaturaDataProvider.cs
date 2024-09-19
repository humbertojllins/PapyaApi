using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using papya_api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using papya_api.ExtensionMethods;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto;

namespace papya_api.DataProvider
{
    public class AssinaturaDataProvider : IAssinaturaDataProvider
    {

        private readonly IConfiguration _configuration;
        public AssinaturaDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object AddAssinatura(Assinatura assinatura)
        {
            int idAssinatura = 0;
            object ret;
            DateTime? referencia = RecuperaReferencia((int)assinatura.idEstabelecimento);
            referencia = referencia == null ? assinatura.data_pagamento : referencia;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                try
                {
                    conexao.Open();
                    var sql = "insert into assinatura (idEstabelecimento,chave_pagamento,cnpj,referencia,data_pagamento,plano,valorpago,statuspagamento,retornopagamento)" +
                        " values(" +
                        "" + assinatura.idEstabelecimento + "," +
                        "'" + assinatura.chave_pagamento + "'," +
                        "'" + assinatura.cnpj + "'," +
                        "'" + ((DateTime)referencia).ToString("yyyy-MM-dd") + "'," +
                        "'" + ((DateTime)assinatura.data_pagamento).ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                        "'" + assinatura.plano + "'," +
                        "'" + assinatura.valorpago + "'," +
                        "'" + assinatura.statuspagamento + "'," +
                        "'" + assinatura.retornopagamento + "');";
                    sql += " select last_insert_id();";
                    idAssinatura = Convert.ToInt32(conexao.ExecuteScalar(sql, commandType: System.Data.CommandType.Text));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return idAssinatura;

            }
        }
        public object UpdateAssinatura(Assinatura assinatura)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                DateTime? validade = RecuperaValidade((int)assinatura.idEstabelecimento, assinatura.data_pagamento, assinatura.plano);
                try
                {
                    conexao.Open();
                    var sql = "update assinatura SET " +
                        " statuspagamento='" + assinatura.statuspagamento + "'," +
                        " plano='" + assinatura.plano + "',";
                    if (assinatura.statuspagamento == "approved")
                    {
                        sql+= " validade='" + ((DateTime)validade).ToString("yyyy-MM-dd HH:mm:ss") + "',";
                        sql+= " valorpago=" + assinatura.valorpago + ",";
                    }
                    sql += " retornopagamento='" + assinatura.retornopagamento + "'" +
                    " where idestabelecimento=" + assinatura.idEstabelecimento +
                    " and chave_pagamento='" + assinatura.chave_pagamento + "'" +
                    " and statuspagamento <> 'approved'";

                    ret = conexao.Execute(sql
                        , commandType: System.Data.CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ret;
        }

        DateTime? RecuperaValidade(int idEstabelecimento, DateTime? dtPagamento, string plano)
        {
            DateTime validade = (DateTime)dtPagamento;
            Assinatura assinatura = GetUltimaAssinatura(idEstabelecimento).Result;
            if (assinatura != null)
            {
                validade = (DateTime)assinatura.validade;
            }
            switch (plano)
            {
                case "mensal":
                    return validade.AddMonths(1);
                case "trimestral":
                    return validade.AddMonths(3);
                case "anual":
                    return validade.AddMonths(12);
            }
            //}
            return null;
        }
        DateTime? RecuperaReferencia(int idEstabelecimento)
        {
            Assinatura assinatura = GetUltimaAssinatura(idEstabelecimento).Result;
            if (assinatura != null)
            {
                DateTime validade = (DateTime)assinatura.validade;
                return validade.AddMonths(1);
            }
            return null;
        }

        public object DeleteAssinatura(int id)
        {
            object ret;
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                conexao.Open();
                var sql = "update assinatura set statuspagamento='Cancelado'" +
                    " where id=" + id.ToString();

                ret = conexao.Execute(sql
                    , commandType: System.Data.CommandType.Text);
                conexao.Close();
            }
            return ret;
        }
        //}
        public async Task<Assinatura> GetAssinatura(int id)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select * ";
                sql += "from assinatura a ";
                sql += " where a.id=" + id.ToString();
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Assinatura>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<Assinatura> GetAssinatura(string chave_pagamento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select * ";
                sql += "from assinatura a ";
                sql += " where a.chave_pagamento='" + chave_pagamento + "'";
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Assinatura>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }
        public async Task<Assinatura> GetUltimaAssinatura(int idEstabelecimento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select * ";
                sql += "from assinatura a ";
                sql += " where idestabelecimento=" + idEstabelecimento;
                sql += " and validade = (select max(validade) from assinatura where idEstabelecimento =" + idEstabelecimento + ")";
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Assinatura>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }
        public async Task<Assinatura> GetAssinaturaPendente(int idEstabelecimento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select * ";
                sql += "from assinatura a ";
                sql += " where idestabelecimento=" + idEstabelecimento;
                sql += " and statuspagamento='created'";
                conexao.Open();
                return await conexao.QuerySingleOrDefaultAsync<Assinatura>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }

        public async Task<IEnumerable<Assinatura>> GetAssinaturas(int idEstabelecimento)
        {
            using (var conexao = new MySqlConnection(ConnectionHelper.GetConnectionString(_configuration)))
            {
                string sql = "select * ";
                sql += "from assinatura a ";
                sql += " where idEstabelecimento=" + idEstabelecimento;
                conexao.Open();
                return await conexao.QueryAsync<Assinatura>(
                sql,
                null,
                commandType: System.Data.CommandType.Text);
            }
        }
    }
}
