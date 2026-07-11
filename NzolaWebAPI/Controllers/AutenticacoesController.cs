using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;

namespace NzolaWebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela autenticação: registo, login, refresh token e logout.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacoesController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly ITokenService _tokenService;
        private readonly IUtilizadorService _utilizadorService;
        private readonly ILogger<AutenticacoesController> _logger;

        public AutenticacoesController(
            ContextoBDNzola contexto,
            ITokenService tokenService,
            IUtilizadorService utilizadorService,
            ILogger<AutenticacoesController> logger
        )
        {
            _contexto = contexto;
            _tokenService = tokenService;
            _utilizadorService = utilizadorService;
            _logger = logger;
        }

        /// <summary>
        /// Regista um novo utilizador na plataforma. Valida dados obrigatórios, verifica unicidade de email e nome de utilizador, e retorna os dados do utilizador criado com token JWT.
        /// </summary>
        /// <param name="registoDto">Objeto com os dados de registo do novo utilizador.</param>
        /// <returns>Mensagem de sucesso do registo.</returns>
        /// <response code="200">Utilizador registado com sucesso.</response>
        /// <response code="400">Dados inválidos ou e-mail já em uso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost("registo")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Autentica o utilizador com email/nomeUtilizador e password. Retorna os dados do utilizador com token JWT e refresh token.
        /// </summary>
        /// <param name="loginDto">Objeto com Email (ou NomeUtilizador) e Password.</param>
        /// <returns>Dados do utilizador autenticado com token JWT e refresh token.</returns>
        /// <response code="200">Login efetuado com sucesso, retorna token e dados do utilizador.</response>
        /// <response code="401">Credenciais inválidas.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Gera um novo token JWT a partir de um refresh token válido.
        /// </summary>
        /// <param name="refreshRequest">Objeto contendo o refresh token.</param>
        /// <returns>Novo token JWT, refresh token e dados do utilizador.</returns>
        /// <response code="200">Novo token gerado com sucesso.</response>
        /// <response code="401">Refresh token inválido ou expirado.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(RefreshTokenResponseDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto refreshRequest)
        {
            try
            {
                var refreshToken = await _tokenService.ValidarRefreshTokenAsync(refreshRequest.RefreshToken);
                if (refreshToken == null)
                    return Unauthorized("Refresh token inválido ou expirado");

                await _tokenService.RevogarRefreshTokenAsync(refreshRequest.RefreshToken);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer refresh token");
                return StatusCode(500, new { erro = "Erro interno ao renovar sessão.", detalhe = ex.Message });
            }
        }

        /// <summary>
        /// Invalida o refresh token do utilizador, efectuando logout.
        /// </summary>
        /// <param name="logoutRequest">Objeto com o ID do utilizador.</param>
        /// <returns>Mensagem de sessão terminada com sucesso.</returns>
        /// <response code="200">Sessão terminada com sucesso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost("logout")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto logoutRequest)
        {
            await _tokenService.RevogarRefreshTokenAsync(logoutRequest.RefreshToken);
            await _contexto.SaveChangesAsync();
            return Ok(new { mensagem = "Sessão terminada com sucesso!" });
        }

        /// <summary>
        /// Envia um e-mail com link para redefinir a palavra-passe, caso o e-mail exista.
        /// </summary>
        /// <param name="dto">Objeto com o e-mail do utilizador</param>
        /// <response code="200">Email de redefinição enviado (se a conta existir)</response>
        /// <response code="400">Email inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("esqueci-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EsqueciPassword([FromBody] EsqueciPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("E-mail inválido.");

            var token = await _utilizadorService.GerarTokenRedefinirPasswordAsync(dto.Email);

            return Ok(new { mensagem = "Se o e-mail existir, receberá um link de redefinição da palavra-passe." });
        }

        /// <summary>
        /// Redefine a palavra-passe utilizando o token enviado por e-mail.
        /// </summary>
        /// <param name="dto">Objeto com o token e a nova palavra-passe</param>
        /// <response code="200">Palavra-passe redefinida com sucesso</response>
        /// <response code="400">Token inválido, expirado ou dados inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("redefinir-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RedefinirPassword([FromBody] RedefinirPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var resultado = await _utilizadorService.RedefinirPasswordAsync(dto.Token, dto.NovaPalavraPasse);

            if (!resultado)
                return BadRequest("Token inválido ou expirado.");

            return Ok(new { mensagem = "Palavra-passe redefinida com sucesso!" });
        }
    }
}
