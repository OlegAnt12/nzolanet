using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class AtualizarUtilizadorDto
    {
        public int Id { get; set; }

        public string NomeCompleto { get; set; }

        public string Email { get; set; }

        public string Biografia { get; set; }

        public string Privacidade { get; set; }

        public string EstadoConta { get; set; }

        public byte[]? FotoPerfil { get; set; }
    }
}
