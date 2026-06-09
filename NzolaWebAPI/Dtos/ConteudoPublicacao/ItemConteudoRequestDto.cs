using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.ConteudoPublicacao
{
    public class ItemConteudoRequestDto
    {
        public int PublicacaoId { get; set; }
        public string Texto { get; set; }
        public IFormFile Ficheiro { get; set; }
        public int Ordem { get; set; }
        public TipoConteudo TipoConteudo { get; set; }
    }
}
