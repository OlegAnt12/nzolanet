using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    public class Utilizador
    {
        public int Id { get; set; }
        public string Genero { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string PalavraPasse { get; set; }
        public NivelAcesso NivelAcesso { get; set; }
        public byte[] FotoPerfil { get; set; }
        public string Biografia { get; set; }
        public EstadoAcesso Privacidade { get; set; }
        public EstadoConta EstadoConta { get; set; }
        public DateTime DataRegistro { get; set; }
        public DateTime DataNascimento { get; set; }

        public List<Seguidor> Seguidores { get; set; } = new List<Seguidor>();
        public List<Publicacao> Publicacoes { get; set; } = new List<Publicacao>();
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<Baze> Bazes { get; set; } = new List<Baze>();
    }
}
