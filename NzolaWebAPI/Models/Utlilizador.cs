using System;
using System.collection.Generic;
using system.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.Models
{
    public class Utilizador {

        public int Id {get; set;}
        public string NomeCompleto{get; set;}
        public string Email {get; set;}
        public string PalavraPasse{ get; set;}
        public NivelAcesso NivelAcesso{get; set;}
        public byte[] FotoPerfil{ get; set;}
        public string Biografia{get; set;}
        public EstadoAcesso Privacidade {get; set}
        public EstadoConta EstadoConta{get; set;}
        public DataTime DataRegistro{get; set;}
        public List<Seguidor> Seguidor {get; set;} = new List<Seguidor>();
        public List<Publicacao> Publicacoes{get; set;} = new List<Publicacao>();
        public List<Comentario> Comentarios {get; set;} = new List<Comentario>();
        public List<Baze> Bazes {get; set;} =new List<Baze>();


      }
    
}