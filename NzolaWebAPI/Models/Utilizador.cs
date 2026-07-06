using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    [Table("tb_Utilizadores")]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(NomeUtilizador), IsUnique = true)]
    public class Utilizador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EnumDataType(typeof(Genero))]
        [Column(TypeName = "nvarchar(10)")]
        public Genero Genero { get; set; }

        [Required]
        [MaxLength(50)]
        public string NomeUtilizador { get; set; } = string.Empty;

        //[RegularExpression(@"^\[A-Z]{2}\[A-Z]{2}$", ErrorMessage = "NomeCompleto inválido")] // Expressão regular para o Nome completo
        public string NomeCompleto { get; set; } = string.Empty;

        //[RegularExpression(@"^\d{9}[A-Z]{2}\d{3}$", ErrorMessage = "E-mail inválido")] // Expressão regular para o E-mail
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        [MinLength(6)]
        public string PalavraPasse { get; set; } = string.Empty; // Expressão regular para o Nome completo

        [Column(TypeName = "nvarchar(8)")]
        [DefaultValue("Normal")]
        public NivelAcesso NivelAcesso { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[]? FotoPerfil { get; set; }
        public string? Biografia { get; set; }

        [Column(TypeName = "nvarchar(8)")]
        [DefaultValue("Publico")]
        public EstadoAcesso Privacidade { get; set; }

        [Column(TypeName = "nvarchar(8)")]
        [DefaultValue("Activa")]
        public EstadoConta EstadoConta { get; set; }
        public DateTime DataRegistro { get; set; } = DateTime.Now;
        public DateTime DataNascimento { get; set; }

        public List<Seguidor> Seguidores { get; set; } = new List<Seguidor>();
        public List<Seguidor> Seguindo { get; set; } = new List<Seguidor>();
        public List<Publicacao> Publicacoes { get; set; } = new List<Publicacao>();
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
        public List<Baze> Bazes { get; set; } = new List<Baze>();
        public bool ConcordaComTermos { get; set; }

        public string? ResetTokenRedefinirPassword { get; set; }
        public DateTime? ResetTokenExpiraEm { get; set; }
    }
}
