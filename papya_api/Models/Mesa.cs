using System.ComponentModel.DataAnnotations;
namespace papya_api.Models
{
    public class Mesa
    {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int fk_id_estabelecimento { get; set; }
        public int? fk_Id_funcionario { get; set; }
        public int? id_funcionario_mesa { get; set; }
        public string nome_funcionario { get; set; }

    }
}
