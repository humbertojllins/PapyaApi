using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace papya_api.Models
{
    public class UsuarioSocial
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
        public string EstabelecimentoNome { get; set; }
        public string EstabelecimentoEndereco { get; set; }
        public float EstabelecimentoLatitude { get; set; }
        public float EstabelecimentoLongitude { get; set; }
        public string EstabelecimentoImagem { get; set; }
        public float DistanciaKm { get; set; }
        public DateTime? DataLeitura { get; set; }
        public int qtdmensagem { get; set; }
        

        public static explicit operator UsuarioSocial(Task<UsuarioSocial> v)
        {
            throw new NotImplementedException();
        }
    }
}
