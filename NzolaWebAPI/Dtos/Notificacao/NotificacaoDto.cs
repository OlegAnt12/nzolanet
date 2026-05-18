using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NzolaWebAPI.DTOs.Notificacao
{
    public class NotificacaoDto
    {
        
        public int Id {get; set;}
        public int UtilizadorId {get; set;}
        public string Tipo {get; set;} ="";
        public int OrigemId {get; set;}
        public string? Mensagem {get; set;}
        public bool Lida {get; set;} 
        public DateTime CriadoEm {get; set;} 
    }
    
}