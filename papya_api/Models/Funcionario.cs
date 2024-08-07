using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Funcionario
    {
        [Key]
        public int ID { get; set; }
        public int ID_USUARIO { get; set; }
        public int ID_ESTABELECIMENTO { get; set; }
        public double SALARIO { get; set; }
        public int ID_TIPOFUNCIONARIO { get; set; }
        public string NomeEstabelecimento { get; set; }
        public string ImagemEstabelecimento { get; set; }

        public static explicit operator Funcionario(Task<Funcionario> v)
        {
            throw new NotImplementedException();
        }
    }
}
