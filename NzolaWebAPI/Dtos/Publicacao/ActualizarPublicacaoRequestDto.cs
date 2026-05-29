using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.DTOs.Comentario;

namespace NzolaWebAPI.DTOs.Publicacao
{
    public class ActualizarPublicacaoRequestDto
    {
        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public DateTime DataAtualizacaoPublicacao { get; set; } = DateTime.Now;
        public List<ConteudoPublicacaoDto> Conteudos { get; set; } = new List<ConteudoPublicacaoDto>();
        public List<ComentarioDto> Comentarios {get; set;} = new List<ComentarioDto>();
    }
}