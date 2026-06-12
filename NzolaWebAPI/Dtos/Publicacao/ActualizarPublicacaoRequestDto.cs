using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Comentario;

namespace NzolaWebAPI.DTOs.Publicacao
{
    public class ActualizarPublicacaoRequestDto
    {
        public string Texto { get; set; } = string.Empty;
        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public DateTime DataAtualizacaoPublicacao { get; set; } = DateTime.Now;
        public List<FicheiroPublicacaoDto> Ficheiros { get; set; } = new List<FicheiroPublicacaoDto>();
        public List<ComentarioDto> Comentarios {get; set;} = new List<ComentarioDto>();
    }
}