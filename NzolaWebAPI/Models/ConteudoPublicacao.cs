using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    public class ConteudoPublicacao
    { 
        public int Id {get; set;}
        public int PublicacaoId { get; set; }
        public string Conteudo {get; set;}
        public TipoConteudo TipoConteudo { get; set; }
        public int Ordem { get; set; }
    }
}