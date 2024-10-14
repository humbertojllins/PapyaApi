using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Categoria
    {
        [Key]
        public int id { get; set; }
        public string descricao { get; set; }
        public int fk_id_estabelecimento { get; set; }
      

        public static explicit operator Categoria(Task<Conta> v)
        {
            throw new NotImplementedException();
        }
    }
}
