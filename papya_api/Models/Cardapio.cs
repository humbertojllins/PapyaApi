using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Cardapio
    {
        [Key]
        public int codigo_item { get; set; }
        public string item_titulo { get; set; }
        public string item_descricao { get; set; }
        public double valor_item { get; set; }
        public int tempo_estimado_min { get; set; }
        public int tempo_estimado_max { get; set; }
        public string imagem { get; set; }
        public int item_id_categoria { get; set; }
        public string categoria { get; set; }
        public int fk_id_estabelecimento { get; set; }
        public int is_cozinha { get; set; }
        public int is_cardapio { get; set; }
        public float desconto { get; set; }


        public static explicit operator Cardapio(Task<Cardapio> v)
        {
            throw new NotImplementedException();

            //Teste
        }
    }
}
