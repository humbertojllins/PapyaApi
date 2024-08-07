using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace papya_api.ExtensionMethods
{
	public static class ConnectionHelper
	{
        private static IConfiguration _configuration;

        //public static ConnectionHelper(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        public static string GetConnectionString(IConfiguration configuration)
        {
            _configuration = configuration;
            MySqlConnectionStringBuilder strConexaoInicial = new MySqlConnectionStringBuilder(_configuration.GetConnectionString("tingledb"));
            //strConexaoInicial.Password = _configuration["Tingle:Senha"];
            return strConexaoInicial.ConnectionString;
        }
    }
}

