using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.ConteudoPublicacao
{
    public class AdicionarConteudoPublicacaRequestDto
    {
        public int PublicacaoId { get; set; }
        public string Conteudo { get; set; }
        public TipoConteudo TipoConteudo { get; set; }
        public int Ordem { get; set; }
    }
}
