using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Comentario
{
    public class ComentarioResultadoDto
    {
        public bool Sucesso { get; set; } = true;
        public string? MensagemErro { get; set; }
        public bool NaoEncontrado { get; set; }
        public ComentarioDto? ComentarioDto { get; set; }
    }
}
