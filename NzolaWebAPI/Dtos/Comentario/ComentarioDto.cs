using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Utilizador;

namespace NzolaWebAPI.DTOs.Comentario
{
    public class ComentarioDto
    {
        public int Id { get; set; }
        public int PublicacaoId { get; set; }
        public UtilizadorSimplificadoDto Comentador { get; set; } = null!;
        public string ConteudoComentario { get; set; } = string.Empty;
        public DateTime DataComentario { get; set; } = DateTime.Now;
        public DateTime DataActualizacao { get; set; }
    }
}
