using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.ConteudoPublicacao
{
    public class ItemConteudoRequestDto
    {
        public int PublicacaoId { get; set; }
        public string Texto { get; set; }
        public IFormFile Ficheiro { get; set; }
        public int Ordem { get; set; }
    }
}
