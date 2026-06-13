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
    }
}
