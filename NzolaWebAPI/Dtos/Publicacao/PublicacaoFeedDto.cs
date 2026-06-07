using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Comentario;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.DTOs.Utilizador;

namespace NzolaWebAPI.DTOs.Publicacao
{
    public class PublicacaoFeedDto
    {
        public int Id { get; set; }
        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public DateTime DataPublicacao { get; set; } = DateTime.Now;

        public AutorPublicacaoDto Autor { get; set; }
        public List<ConteudoPublicacaoDto> Conteudos { get; set; } =
            new List<ConteudoPublicacaoDto>();
        public List<ComentarioDto> Comentarios { get; set; } = new List<ComentarioDto>();
    }
}
