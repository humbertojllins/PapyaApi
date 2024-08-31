using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class ContaDetalhe
    {
        [Key]
        public int NUM_CONTA { get; set; }
        public int USUARIO_ID { get; set; }
        public int ID_USUARIO_CONTA { get; set; }
        public string NOME_USUARIO { get; set; }
        public string CPF_USUARIO { get; set; }
        public string DESC_MESA { get; set; }
        public double VALOR_TOTAL_CONTA { get; set; }
        public double VALOR_CONTA_USUARIO { get; set; }
        public string ITEM_CATEGORIA { get; set; }
        public string ITEM_TITULO { get; set; }
        public string ITEM_DESCRICAO { get; set; }
        public double ITEM_QTD { get; set; }
        public double ITEM_VALOR { get; set; }
        public string ITEM_IMAGEM { get; set; }
        public string ITEM_STATUS { get; set; }
        public int? ITEM_IS_COZINHA { get; set; }
        public int ITEM_PEDIDOITEM_ID { get; set; }
        public string NOME_FUNCIONARIO { get; set; }
        public string STATUS_CONTA { get; set; }
        public string STATUS_CONTA_USUARIO { get; set; }
        public float ITEM_DESCONTO { get; set; }
        public int PEDIDO_ID { get; set; }
        public DateTime PEDIDO_DATAHORA { get; set; }
        public DateTime ITEM_PREV_MIN { get; set; }
        public DateTime ITEM_PREV_MAX { get; set; }
        public double VALOR_PAGO_CONTA_USUARIO { get; set; }
        public string USUARIO_MEIO_PAGAMENTO { get; set; }
        public int USUARIO_ABRIU_CONTA { get; set; }


        public static explicit operator ContaDetalhe(Task<ContaDetalhe> v)
        {
            throw new NotImplementedException();
        }
    }
}
