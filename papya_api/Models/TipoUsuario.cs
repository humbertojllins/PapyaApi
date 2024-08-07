using System.ComponentModel.DataAnnotations;
namespace papya_api.Models
{
    public class TipoUsuario
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Abrev { get; set; }
        public string Status { get; set; }
    }
}
