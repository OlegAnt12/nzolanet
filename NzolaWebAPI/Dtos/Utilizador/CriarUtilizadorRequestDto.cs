using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class CriarUtilizadorRequestDto
    {
        //nome e email

        public string NomeCompleto { get; set; } = string.Empty;
        public string NomeUtilizador { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? FotoPerfil { get; set; }
        public string PalavraPasse { get; set; } = string.Empty;
        public Genero genero { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
