using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    [Table("tb_Denuncias")]
    public class Denuncia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TipoEntidade TipoEntidade { get; set; }

        [Required]
        public int IdEntidade { get; set; }

        [Required]
        [MaxLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Descricao { get; set; }

        [Required]
        public int DenuncianteId { get; set; }

        [ForeignKey("DenuncianteId")]
        public Utilizador? Denunciante { get; set; }

        public DateTime DataDenuncia { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public EstadoDenuncia EstadoDenuncia { get; set; } = EstadoDenuncia.Pendente;
    }
}
