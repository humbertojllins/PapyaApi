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
        public int id_usuario_conta { get; set; }
        public int id_item { get; set; }
        public double qtd_item { get; set; }
        public int fk_id_mesa { get; set; }
        public float desconto { get; set; }

        public static explicit operator Pedido(Task<Pedido> v)
        {
            throw new NotImplementedException();
        }
    }

    public class UltimoPedido
    {
        [Key]
        public string item_titulo { get; set; }
        public string item_descricao { get; set; }
        public int id_item { get; set; }
        public double valor_item { get; set; }
        public double qtd_item { get; set; }
        public string imagem { get; set; }
        public string categoria { get; set; }
        public string nome { get; set; }
        public string mesa { get; set; }

        public static explicit operator UltimoPedido(Task<UltimoPedido> v)
        {
            throw new NotImplementedException();
        }
    }
}
