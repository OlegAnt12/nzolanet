using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _chave;

        public TokenService(IConfiguration config)
        {
            _config = config;
        
            _chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
        }

        public string CriarToken(Utilizador utilizador)
        {
           
            var claims = new List<Claim>
            {
                new Claim("utilizadorId", utilizador.Id.ToString()),
                new Claim("email", utilizador.Email),
                new Claim("name", utilizador.NomeUtilizador)
            };

            var credenciais = new SigningCredentials(_chave, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7), 
                SigningCredentials = credenciais,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}