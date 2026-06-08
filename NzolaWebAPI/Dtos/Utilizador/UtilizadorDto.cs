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
        public String Email { get; set; }
        public String Biografia { get; set; }
        public EstadoAcesso Privacidade { get; set; }
        public EstadoConta EstadoConta { get; set; }
        public byte[] FotoPerfil { get; set; }

        public String Genero { get; set; }
        public DateTime DataNascimento { get; set; }
        
    }
}