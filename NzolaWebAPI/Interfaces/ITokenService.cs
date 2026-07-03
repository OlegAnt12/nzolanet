using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface ITokenService
    {
        string CriarToken(Utilizador utilizador);
        RefreshToken GerarRefreshToken(int utilizadorId);
        Task<RefreshToken?> ValidarRefreshTokenAsync(string token);
        Task RevogarRefreshTokenAsync(string token);
    }
}