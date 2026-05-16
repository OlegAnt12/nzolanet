using System;

namespace NzolaWebAPI.Models
{
    public class Notificacao
    {
        public int Id {get; set;}
        public int UtilizadorId {get; set;}
        public string Tipo {get; set;} ="";
        public int OrigemId {get; set;}
        public string? Mensagem {get; set;}
        public bool Lida {get; set;} =false;
        public DateTime CriadoEm {get; set;} = DateTime.Now;
    }
}
