using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class PedidoItem
    {
        [Key]
        public int ID { get; set; }
        public int ID_PEDIDO { get; set; }
        public int FK_STATUS_ID { get; set; }
        public int FK_itens_ID { get; set; }
        public int QTD { get; set; }
        public float DESCONTO { get; set; }

        public static explicit operator PedidoItem(Task<PedidoItem> v)
        {
            throw new NotImplementedException();
        }
    }
}
