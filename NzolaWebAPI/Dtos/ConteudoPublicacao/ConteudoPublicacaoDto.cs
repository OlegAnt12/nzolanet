using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.ConteudoPublicacao
{
    public class ConteudoPublicacaoDto
    {
        public int Id {get; set;}
        public int PublicacaoId { get; set; }
        public string Conteudo {get; set;}
        public TipoConteudo TipoConteudo { get; set; }
        public int Ordem { get; set; }
    }
}