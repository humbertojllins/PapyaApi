using System.ComponentModel.DataAnnotations;
namespace papya_api.Models
{
    public class StatusItem
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
    }
}
