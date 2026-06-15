using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class UtilizadorSimplificadoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? FotoPerfil { get; set; }
        public bool JaSegues { get; set; } // Estado de seguir AQUI
    }
}
