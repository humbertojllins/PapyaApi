using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Funcionario
    {
        [Key]
        public int id { get; set; }
        public int id_usuario { get; set; }
        public int id_estabelecimento { get; set; }
        public double salario { get; set; }
        public int id_tipofuncionario { get; set; }
        public string nomeestabelecimento { get; set; }
        public string imagemestabelecimento { get; set; }

        public static explicit operator Funcionario(Task<Funcionario> v)
        {
            throw new NotImplementedException();
        }
    }
}
