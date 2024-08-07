using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Notificacao
    {
        [Key]
        public int Id { get; set; }
        public int Fkestabelecimento { get; set; }
        public string Client { get; set; }
        public string EndPoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
        

        public static explicit operator Notificacao(Task<Notificacao> v)
        {
            throw new NotImplementedException();
        }
    }
}
