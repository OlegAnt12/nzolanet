using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NzolaWebAPI.Data;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _chave;
        private readonly ContextoBDNzola _context;

        public TokenService(IConfiguration config, ContextoBDNzola context)
        {
            _config = config;
            _context = context;
            _chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
        }

        public string CriarToken(Utilizador utilizador)
        {
            var claims = new List<Claim>
            {
                new Claim("utilizadorId", utilizador.Id.ToString()),
                new Claim("email", utilizador.Email),
                new Claim("name", utilizador.NomeUtilizador),
                new Claim(ClaimTypes.Role, utilizador.NivelAcesso == NzolaWebAPI.Models.Enums.NivelAcesso.Admin ? "Admin" : "User"),
                new Claim("nivelAcesso", ((int)utilizador.NivelAcesso).ToString()),
            };

            var credenciais = new SigningCredentials(
                _chave,
                SecurityAlgorithms.HmacSha512Signature
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = credenciais,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public RefreshToken GerarRefreshToken(int utilizadorId)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                UtilizadorId = utilizadorId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
            };

            _context.RefreshTokens.Add(refreshToken);
            return refreshToken;
        }

        public async Task<RefreshToken?> ValidarRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.Utilizador)
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.IsActive);
        }

        public async Task RevogarRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken != null)
            {
                refreshToken.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
