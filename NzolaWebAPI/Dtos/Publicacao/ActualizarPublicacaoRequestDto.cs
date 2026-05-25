using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Publicacao
{
    public class ActualizarPublicacaoRequestDto
    {
        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public DateTime DataAtualizacaoPublicacao { get; set; } = DateTime.Now;
        public List<ConteudoPublicacao> Conteudos { get; set; } = new List<ConteudoPublicacao>();
        public List<Comentario> Comentarios {get; set;} = new List<Comentario>();
    }
}