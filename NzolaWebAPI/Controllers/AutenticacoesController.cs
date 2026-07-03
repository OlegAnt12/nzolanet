using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacoesController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly ITokenService _tokenService;

        public AutenticacoesController(ContextoBDNzola contexto, ITokenService tokenService)
        {
            _contexto = contexto;
            _tokenService = tokenService;
        }

        [HttpPost("registo")]
        public async Task<IActionResult> Registar([FromBody] CriarUtilizadorRequestDto registoDto)
        {
            if (!registoDto.ConcordaComTermos)
                return BadRequest("É necessário concordar com os termos da NzolaNet.");

            var emailExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Email.ToLower() == registoDto.Email.ToLower()
            );

            if (emailExiste)
                return BadRequest("Este e-mail já está a ser utilizado");

            var novoUtilizador = registoDto.ToUtilizadorFromCriarDto();
            novoUtilizador.DataRegistro = DateTime.UtcNow;

            _contexto.Utilizadores.Add(novoUtilizador);
            await _contexto.SaveChangesAsync();

            return Ok(new { mensagem = "Utilizador registado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var identificador = loginDto.Identificador.ToLower();

            var utilizador = await _contexto.Utilizadores.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == identificador ||
                u.NomeUtilizador.ToLower() == identificador
            );

            if (utilizador == null || utilizador.PalavraPasse != loginDto.PalavraPasse)
                return Unauthorized("E-mail, nome de utilizador ou palavra-passe incorretos");

            var token = _tokenService.CriarToken(utilizador);
            var refreshToken = _tokenService.GerarRefreshToken(utilizador.Id);
            await _contexto.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Login efetuado com sucesso!",
                token,
                refreshToken = refreshToken.Token,
                utilizador = utilizador.ToUtilizadorDto(),
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var refreshToken = await _tokenService.ValidarRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null)
                return Unauthorized("Refresh token inválido ou expirado");

            await _tokenService.RevogarRefreshTokenAsync(request.RefreshToken);

            var novoToken = _tokenService.CriarToken(refreshToken.Utilizador);
            var novoRefreshToken = _tokenService.GerarRefreshToken(refreshToken.UtilizadorId);
            await _contexto.SaveChangesAsync();

            return Ok(new RefreshTokenResponseDto
            {
                Token = novoToken,
                RefreshToken = novoRefreshToken.Token,
                Utilizador = refreshToken.Utilizador.ToUtilizadorDto(),
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            await _tokenService.RevogarRefreshTokenAsync(request.RefreshToken);
            await _contexto.SaveChangesAsync();
            return Ok(new { mensagem = "Sessão terminada com sucesso!" });
        }
    }
}
