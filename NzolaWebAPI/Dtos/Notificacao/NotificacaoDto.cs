using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Models.Enums;


namespace NzolaWebAPI.DTOs.Notificacao
{
    public class NotificacaoDto
    {
        
        public int Id {get; set;}
        public int UtilizadorId {get; set;}
        public TipoNotificacao Tipo {get; set;}
        public UtilizadorSimplificadoDto? UtilizadorNotificacao { get; set; }

        public int OrigemId { get; set; }
        public UtilizadorSimplificadoDto? UtilizadorResponsavel { get; set; }
        public string? Mensagem {get; set;}
        public bool Lida {get; set;} 
        public DateTime CriadoEm {get; set;} 
    }
    
}