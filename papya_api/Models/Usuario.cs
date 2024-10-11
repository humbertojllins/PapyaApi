using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public int? Id_Tipo_Usuario { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Nascimento { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public string Imagem { get; set; }
        public int? Resetar_Senha { get; set; }
        public string Telefone { get; set; }
        public string StatusSocial { get; set; }
        public string chaveNotificacao { get; set; }
        

        public static explicit operator Usuario(Task<Usuario> v)
        {
            throw new NotImplementedException();
        }
    }
}
