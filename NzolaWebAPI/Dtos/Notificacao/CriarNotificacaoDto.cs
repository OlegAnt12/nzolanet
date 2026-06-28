using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models.Enums;
using NzolaWebAPI.Models;
using NzolaWebAPI.DTOs.Utilizador;

namespace NzolaWebAPI.DTOs.Notificacao
{
    public class CriarNotificacaoDto
    {
        public int UtilizadorId { get; set; }
        public TipoNotificacao Tipo { get; set; }

        public int OrigemId { get; set; }
        public string? Mensagem { get; set; }

    }
}