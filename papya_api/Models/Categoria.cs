using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Imagem { get; set; }
        public int Fk_Id_estabelecimento { get; set; }
      

        public static explicit operator Categoria(Task<Conta> v)
        {
            throw new NotImplementedException();
        }
    }
}
