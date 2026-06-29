using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    [Table("tb_Notificacoes")]
    public class Notificacao
    {
        [Key]
        public int Id {get; set;}
        public int UtilizadorId {get; set;}

        [ForeignKey("UtilizadorId")]
        public Utilizador? UtilizadorNotificacao {get; set;}

        [Column(TypeName = "nvarchar(10)")]
        public TipoNotificacao Tipo {get; set;}

        public int OrigemId {get; set;}
        [ForeignKey("OrigemId")]
        public Utilizador? UtilizadorResponsavel {get; set;}

        public string? Mensagem {get; set;}
        public bool Lida {get; set;} = false;
        public DateTime CriadoEm {get; set;} = DateTime.Now;
    }
}
