using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.DTOs.Denuncia;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Controllers
{
    /// <summary>
    /// Controlador para criação e consulta de denúncias no sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DenunciasController : ControllerBase
    {
        private readonly IDenunciaService _denunciaService;

        public DenunciasController(IDenunciaService denunciaService)
        {
            _denunciaService = denunciaService;
        }

        /// <summary>
        /// Regista uma nova denúncia contra uma publicação, comentário ou utilizador.
        /// </summary>
        /// <param name="dto">Dados da denúncia.</param>
        /// <returns>Mensagem de confirmação da denúncia registada.</returns>
        /// <response code="201">Denúncia registada com sucesso.</response>
        /// <response code="400">Dados inválidos ou motivo obrigatório.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarDenuncia([FromBody] CriarDenunciaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Motivo))
                return BadRequest("O motivo da denúncia é obrigatório.");

            if (dto.DenuncianteId <= 0)
                return BadRequest("Denunciante inválido.");

            var resultado = await _denunciaService.CriarDenunciaAsync(dto);
            if (resultado == null)
                return BadRequest("Não foi possível registar a denúncia.");

            return Ok(new { mensagem = "Denúncia registada com sucesso!" });
        }

        /// <summary>
        /// Lista todas as denúncias registadas no sistema.
        /// </summary>
        /// <returns>Lista de denúncias.</returns>
        /// <response code="200">Lista de denúncias retornada com sucesso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarDenuncias()
        {
            var denuncias = await _denunciaService.ListarTodasAsync();
            return Ok(denuncias);
        }

        /// <summary>
        /// Lista as denúncias de uma entidade específica (publicação, comentário ou utilizador).
        /// </summary>
        /// <param name="tipoEntidade">Tipo da entidade denunciada (0=Publicação, 1=Comentário, 2=Utilizador)</param>
        /// <param name="idEntidade">ID da entidade denunciada</param>
        /// <returns>Lista de denúncias da entidade especificada.</returns>
        /// <response code="200">Lista de denúncias retornada com sucesso.</response>
        /// <response code="400">Parâmetros inválidos.</response>
        /// <response code="404">Entidade não encontrada.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet("entidade/{tipoEntidade}/{idEntidade}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarDenunciasPorEntidade(TipoEntidade tipoEntidade, int idEntidade)
        {
            var denuncias = await _denunciaService.ListarPorEntidadeAsync(tipoEntidade, idEntidade);
            return Ok(denuncias);
        }
    }
}
