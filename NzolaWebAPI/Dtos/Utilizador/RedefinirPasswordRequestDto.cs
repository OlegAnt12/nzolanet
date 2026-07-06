using System.ComponentModel.DataAnnotations;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class RedefinirPasswordRequestDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NovaPalavraPasse { get; set; } = string.Empty;
    }
}
