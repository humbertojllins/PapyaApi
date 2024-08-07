using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class FuncionarioMesa
    {
        [Key]
        public int Id { get; set; }
        public int Fk_Id_funcionario { get; set; }
        public int Fk_Id_mesa { get; set; }


        public static explicit operator FuncionarioMesa(Task<FuncionarioMesa> v)
        {
            throw new NotImplementedException();
        }
    }
}
