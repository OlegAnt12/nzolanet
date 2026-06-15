using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class UtilizadorDto
    {
        public int Id { get; set; }
        public String NomeCompleto { get; set; }
        public String NomeUtilizador { get; set; }
        public String Email { get; set; }
        public String Biografia { get; set; }
        public EstadoAcesso Privacidade { get; set; }
        public EstadoConta EstadoConta { get; set; }
        public string? FotoPerfil { get; set; }

        public Genero Genero { get; set; }
        public DateTime DataNascimento { get; set; }
        public int Seguidores { get; set; } // Quantos seguem ele
        public int Seguindo { get; set; } // Quantos ele segue (NOVO)
        public int Publicacoes { get; set; }
        public bool JaSegues { get; set; }
    }
}
