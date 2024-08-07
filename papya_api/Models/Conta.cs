using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Conta
    {
        [Key]
        public int Id { get; set; }
        public double Total { get; set; }
        public int idMEsa { get; set; }
        public int idStatus { get; set; }
        public DateTime data_hora { get; set; }

        public static explicit operator Conta(Task<Conta> v)
        {
            throw new NotImplementedException();
        }
    }
}
