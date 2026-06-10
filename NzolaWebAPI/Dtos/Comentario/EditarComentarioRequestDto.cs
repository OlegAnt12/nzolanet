using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Comentario
{
    public class EditarComentarioRequestDto
    {
        public string ConteudoComentario { get; set; } = string.Empty;
        public DateTime DataActualizacao { get; set; } = DateTime.Now;
    }
}
