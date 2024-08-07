using System.ComponentModel.DataAnnotations;
namespace papya_api.Models
{
    public class MeioPagamento
    {
        [Key]
        public int Id_Meio_Pagamento { get; set; }
        public string Descricao { get; set; }
    }
}
