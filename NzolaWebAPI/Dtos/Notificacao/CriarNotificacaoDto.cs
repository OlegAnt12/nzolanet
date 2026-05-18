using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Notificacao
{
    public class CriarNotificacaoDto
    {
        public int UtilizadorId { get; set;}
        public string Tipo { get; set;} = "";
        public int OrigemId { get; set;}
        public string? Mensagem { get; set;}
        
    }
}