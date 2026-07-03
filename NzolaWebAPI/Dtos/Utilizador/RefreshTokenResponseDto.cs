using NzolaWebAPI.Models;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class RefreshTokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UtilizadorDto Utilizador { get; set; } = null!;
    }
}
