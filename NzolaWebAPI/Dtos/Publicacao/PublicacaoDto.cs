using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.ConteudoPublicacao;

namespace NzolaWebAPI.DTOs.Publicacao
{
    public class PublicacaoDto
    {
        public int Id { get; set; }
        public int AutorId { get; set; }
        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public DateTime DataPublicacao { get; set; } = DateTime.Now;
        public List<ConteudoPublicacaoDto> Conteudos { get; set; } = new List<ConteudoPublicacaoDto>();
    }
}