using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Services
{
    public class UtilizadorService : IUtilizadorService
    {
        private readonly IUtilizadorRepository _utilizadorRepository;
        private readonly ITokenService _tokenService;

        public UtilizadorService(IUtilizadorRepository utilizadorRepository, ITokenService tokenService)
        {
            _utilizadorRepository = utilizadorRepository;
            _tokenService = tokenService;
        }

        public async Task<string?> LoginAsync(string email, string palavraPasse)
        {
            var utilizador = await _utilizadorRepository.ObterPorEmailAsync(email);

            if (utilizador == null) return null;

            if (utilizador.PalavraPasse != palavraPasse) return null;

            return _tokenService.CriarToken(utilizador);
        }

        public async Task<bool> RegistarAsync(Utilizador utilizador, string palavraPasse)
        {
            if (await _utilizadorRepository.ExisteEmailAsync(utilizador.Email))
            {
                return false; 
            }

            utilizador.PalavraPasse = palavraPasse;

            await _utilizadorRepository.AdicionarAsync(utilizador);

            return await _utilizadorRepository.SalvarAlteracoesAsync();
        }
    }
}