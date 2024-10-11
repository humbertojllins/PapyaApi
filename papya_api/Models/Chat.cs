using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace papya_api.Models
{
    public class Chat
    {
        [Key]
        public int id { get; set; }
        public DateTime datahora { get; set; }
        public int usuariofrom { get; set; }
        public int usuarioto { get; set; }
        public int usuariotoAutorizacao { get; set; }
        public string mensagem { get; set; }
        public string mensagens { get; set; }
        public DateTime? dataleiturafrom { get; set; }
        public DateTime? dataleiturato { get; set; }

        public List<Mensagem> listamensagens { get; set; }
    }
    public class Mensagem
    {
        [Key]
        public string id { get; set; }
        public DateTime data { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string imgFrom { get; set; }
        public string imgTo { get; set; }
        public string mensagem { get; set; }
    }
}
