using System;
using MySql.Data.MySqlClient;
namespace papya_api.Models
{
    public class ConexaoDTP
    {
        //public MySqlConnection ConexaoDTP_Conn = new MySqlConnection("Server=191.252.224.218;Port=3306;Database=tingle_db;Uid=user_site;Pwd=3A-D4u6C%pRI!\":");

        public string StrConexao 
        { 
            get 
            {
                   return "Server=191.252.224.218;Port=3306;Database=tingle_db;Uid=user_site;Pwd=3A-D4u6C%pRI!\":";
            } 
        }

    }
}
