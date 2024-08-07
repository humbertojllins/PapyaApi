using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Promocoes
    {
        [Key]
        public int ID_ESTABELECIMENTO { get; set; }
        public string NOME { get; set; }
        public string CNPJ { get; set; }
        public string ENDERECO { get; set; }
        public string NUMERO { get; set; }
        public float LATITUDE { get; set; }
        public float LONGITUDE { get; set; }
        public string IMAGEM { get; set; }
        public string TIPO_ESTABELECIMENTO { get; set; }
        public int ID_PROMOCAO { get; set; }
        public string DESCRICAO { get; set; }
        public string DATA_VIGENCIA { get; set; }
        public string IMAGEM_PROMOCAO { get; set; }
        public int VALIDADE { get; set; }
        public float DISTANCIA_KM { get; set; }
        public float DESCONTO { get; set; }
        public int QTD_ITEM { get; set; }
        public int CODIGO_ITEM { get; set; }
        public string TITULO { get; set; }
        public string DESCRICAO_ITEM { get; set; }
        public float VALOR { get; set; }
        public int TEMPO_ESPERADO_MIN { get; set; }
        public int TEMPO_ESPERADO_MAX { get; set; }
        public string IMAGEM_ITEM { get; set; }
        public int STATUS { get; set; }
        public static explicit operator Promocoes(Task<Cardapio> v)
        {
            throw new NotImplementedException();
        }
    }
}