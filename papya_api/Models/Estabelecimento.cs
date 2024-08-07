using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Estabelecimento
    {
        [Key]
        public int Id_Estabelecimento { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Cnpj { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int fk_Tipo_Estabelecimento_id { get; set; }
        public string Imagem { get; set; }
        public float Distancia_km { get; set; }
        public string Senha { get; set; }
        public int? Resetar_Senha { get; set; }

        public static explicit operator Estabelecimento(Task<Estabelecimento> v)
        {
            throw new NotImplementedException();
        }
    }
}
