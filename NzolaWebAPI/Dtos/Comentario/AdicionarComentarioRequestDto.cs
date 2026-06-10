using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Comentario
{
    public class AdicionarComentarioRequestDto
    {
        public int PublicacaoId { get; set; }
        public int ComentadorId { get; set; }
        public string ConteudoComentario { get; set; }
    }
}
