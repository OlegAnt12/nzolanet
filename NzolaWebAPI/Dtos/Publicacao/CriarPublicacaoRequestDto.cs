using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.DTOs.Comentario;

namespace NzolaWebAPI.DTOs.Publicacao
{
    public class CriarPublicacaoRequestDto
    {
        public int AutorId { get; set; }
        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public List<AdicionarConteudoPublicacaoRequestDto> Conteudos { get; set; } = new List<AdicionarConteudoPublicacaoRequestDto>();
        public List<ComentarioDto> Comentarios { get; set; } = new List<ComentarioDto>();
    }
}