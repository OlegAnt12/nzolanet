using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class AutorPublicacaoDto
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public byte?[] FotoPerfil { get; set; }
    }
}
