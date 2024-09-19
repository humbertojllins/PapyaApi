using System;
using System.ComponentModel.DataAnnotations;
namespace papya_api.Models
{
    public class Assinatura
    {
        [Key]
        public int id { get; set; }
        public int? idEstabelecimento { get; set; }
        public string chave_pagamento { get; set; }
        public string cnpj { get; set; }
        public DateTime? referencia { get; set; }
        public DateTime? data_pagamento { get; set; }
        public string? plano { get; set; }
        public DateTime? validade { get; set; }
        public double? valorpago { get; set; }
        public string? statuspagamento { get; set; }
        public string retornopagamento { get; set; }
    }

}
