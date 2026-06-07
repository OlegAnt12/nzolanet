using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class AutorPublicacaoDto
    {
        public int Id { get; set; }
        public string NomeUtilizador { get; set; }
        public byte[] FotoPerfil { get; set; }
    }
}