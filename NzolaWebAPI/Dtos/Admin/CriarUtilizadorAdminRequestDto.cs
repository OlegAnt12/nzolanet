using System.ComponentModel.DataAnnotations;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Dtos.Admin
{
    public class CriarUtilizadorAdminRequestDto
    {
        [Required]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NomeUtilizador { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string PalavraPasse { get; set; } = string.Empty;

        public Genero Genero { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
