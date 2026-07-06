using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Services
{
    public class UtilizadorService : IUtilizadorService
    {
        private readonly IUtilizadorRepository _utilizadorRepository;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly ISeguidorRepository _seguidorRepository;
        private readonly IPublicacaoRepository _publicacaoRepository;

        public UtilizadorService(
            IUtilizadorRepository utilizadorRepository,
            IEmailService emailService,
            ITokenService tokenService,
            ISeguidorRepository seguidorRepository,
            IPublicacaoRepository publicacaoRepository
        )
        {
            _utilizadorRepository = utilizadorRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _seguidorRepository = seguidorRepository;
            _publicacaoRepository = publicacaoRepository;
        }

        public async Task<string?> LoginAsync(string email, string palavraPasse)
        {
            var utilizador = await _utilizadorRepository.ObterPorEmailAsync(email);

            if (utilizador == null)
                return null;

            if (utilizador.PalavraPasse != palavraPasse)
                return null;

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

            await _emailService.EnviarEmailConfirmacaoAsync(
                utilizador.Email,
                utilizador.NomeCompleto
            );

            return await _utilizadorRepository.SalvarAsync();
        }

        public async Task<Utilizador?> AtualizarPerfilAsync(
            int utilizadorId,
            ActualizarPerfilRequestDto dto
        )
        {
            // 1. Procura o utilizador na base de dados
            var utilizador = await _utilizadorRepository.ObterPorIdAsync(utilizadorId);
            if (utilizador == null)
                return null;

            // 2. Atualiza os campos textuais
            utilizador.NomeCompleto = dto.NomeCompleto;
            utilizador.Biografia = dto.Biografia;

            // 3. Converte a imagem recebida do Form para byte[]
            if (dto.NovaFoto != null && dto.NovaFoto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.NovaFoto.CopyToAsync(memoryStream);
                    utilizador.FotoPerfil = memoryStream.ToArray(); // Grava os bytes limpos no SQL Server
                }
            }

            // 4. Grava as alterações
            await _utilizadorRepository.SalvarAsync();
            return utilizador;
        }

        public async Task<UtilizadorDto?> ObterPorIdServiceAsync(
            int id,
            int? utilizadorLogadoId = null
        )
        {
            var utilizador = await _utilizadorRepository.ObterPorIdAsync(id);
            if (utilizador == null)
                return null;

            var utilizadorDto = utilizador.ToUtilizadorDto();

            if (utilizadorLogadoId.HasValue && utilizadorLogadoId.Value != id)
            {
                var relacao = await _seguidorRepository.ObterPorRelacaoAsync(
                    utilizadorLogadoId.Value,
                    id
                );
                utilizadorDto.JaSegues = relacao != null;
            }
            else if (utilizadorLogadoId.HasValue && utilizadorLogadoId.Value == id)
            {
                utilizadorDto.JaSegues = false;
            }

            var seguidores = await _seguidorRepository.ContarSeguidoresAsync(id);
            var seguindo = await _seguidorRepository.ContarSeguindoAsync(id);
            var publicacoes = await _publicacaoRepository.ContarPorUtilizadorAsync(id);
            utilizadorDto.Seguidores = seguidores;
            utilizadorDto.Seguindo = seguindo;
            utilizadorDto.Publicacoes = publicacoes;

            return utilizadorDto;
        }

        public async Task<string?> GerarTokenRedefinirPasswordAsync(string email)
        {
            var utilizador = await _utilizadorRepository.ObterPorEmailAsync(email);
            if (utilizador == null) return null;

            var token = Convert.ToHexString(Guid.NewGuid().ToByteArray()) +
                        Convert.ToHexString(Guid.NewGuid().ToByteArray());

            utilizador.ResetTokenRedefinirPassword = token;
            utilizador.ResetTokenExpiraEm = DateTime.UtcNow.AddHours(1);

            await _utilizadorRepository.SalvarAsync();

            await _emailService.EnviarEmailRedefinirPasswordAsync(
                utilizador.Email,
                utilizador.NomeCompleto,
                token
            );

            return token;
        }

        public async Task<bool> RedefinirPasswordAsync(string token, string novaPalavraPasse)
        {
            var utilizador = await _utilizadorRepository.ObterPorTokenRedefinirPasswordAsync(token);
            if (utilizador == null) return false;

            if (utilizador.ResetTokenExpiraEm == null ||
                utilizador.ResetTokenExpiraEm < DateTime.UtcNow)
                return false;

            utilizador.PalavraPasse = novaPalavraPasse;
            utilizador.ResetTokenRedefinirPassword = null;
            utilizador.ResetTokenExpiraEm = null;

            return await _utilizadorRepository.SalvarAsync();
        }

        public async Task<object> ObterEstatisticasAsync(int id)
        {
            var seguidores = await _seguidorRepository.ContarSeguidoresAsync(id);
            var seguindo = await _seguidorRepository.ContarSeguindoAsync(id);
            var publicacoes = await _publicacaoRepository.ContarPorUtilizadorAsync(id);

            return new
            {
                seguidores,
                seguindo,
                publicacoes,
            };
        }
    }
}
