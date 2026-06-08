using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class CriarUtilizadorRequestDto
    {
        //nome e email

        public String NomeCompleto { get ; set; }
        public String Email { get; set; }
        public byte [] FotoPerfil { get; set; }
        public String PalavraPasse { get; set; }
        public String Genero { get; set; }
        public DateTime DataNascimento { get; set; }
        
    }
}