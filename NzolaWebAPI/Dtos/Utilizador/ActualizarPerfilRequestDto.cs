using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class ActualizarPerfilRequestDto
    {
        public string NomeCompleto { get; set; } = string.Empty;

        public IFormFile? NovaFoto { get; set; }

        /*public Genero Genero { get; set; }

        public string NomeUtilizador { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public NivelAcesso NivelAcesso { get; set; }*/

        public string? Biografia { get; set; }

        /*public EstadoAcesso Privacidade { get; set; }

        public EstadoConta EstadoConta { get; set; }
        public DateTime DataNascimento { get; set; }*/
    }
}
