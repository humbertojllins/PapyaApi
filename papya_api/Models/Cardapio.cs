using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Cardapio
    {
        [Key]
        public int CODIGO_ITEM { get; set; }
        public string ITEM_TITULO { get; set; }
        public string ITEM_DESCRICAO { get; set; }
        public double VALOR_ITEM { get; set; }
        public int TEMPO_ESTIMADO_MIN { get; set; }
        public int TEMPO_ESTIMADO_MAX { get; set; }
        public string IMAGEM { get; set; }
        public int ITEM_ID_CATEGORIA { get; set; }
        public string CATEGORIA { get; set; }
        public int FK_ID_ESTABELECIMENTO { get; set; }
        public int IS_COZINHA { get; set; }
        public int IS_CARDAPIO { get; set; }
        public float DESCONTO { get; set; }


        public static explicit operator Cardapio(Task<Cardapio> v)
        {
            throw new NotImplementedException();

            //Teste
        }
    }
}
