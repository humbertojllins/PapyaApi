using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class TipoEstabelecimento
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Imagem { get; set; }


        public static explicit operator TipoEstabelecimento(Task<Estabelecimento> v)
        {
            throw new NotImplementedException();
        }
    }
}
