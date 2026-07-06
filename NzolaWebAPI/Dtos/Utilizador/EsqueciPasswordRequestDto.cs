using System.ComponentModel.DataAnnotations;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class EsqueciPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
