using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using static Mysqlx.Datatypes.Scalar.Types;

namespace papya_api.Models
{
    public class Pedido
    {
        [Key]
        public int ID_USUARIO_CONTA { get; set; }
        public int ID_ITEM { get; set; }
        public double QTD_ITEM { get; set; }
        public int FK_ID_MESA { get; set; }
        public float DESCONTO { get; set; }

        public static explicit operator Pedido(Task<Pedido> v)
        {
            throw new NotImplementedException();
        }
    }

    public class UltimoPedido
    {
        [Key]
        public string ITEM_TITULO { get; set; }
        public string ITEM_DESCRICAO { get; set; }
        public int ID_ITEM { get; set; }
        public double VALOR_ITEM { get; set; }
        public double QTD_ITEM { get; set; }
        public string IMAGEM { get; set; }
        public string CATEGORIA { get; set; }
        public string NOME { get; set; }
        public string MESA { get; set; }

        public static explicit operator UltimoPedido(Task<UltimoPedido> v)
        {
            throw new NotImplementedException();
        }
    }
}
