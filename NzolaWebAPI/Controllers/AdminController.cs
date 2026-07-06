using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Dtos.Admin;
using NzolaWebAPI.DTOs.Denuncia;
using NzolaWebAPI.DTOs.Publicacao;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Controllers
{
    /// <summary>
    /// Controlador administrativo com endpoints para dashboard, listagem de utilizadores, publicações e denúncias.
    /// </summary>
    [Route("api/[Controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IDenunciaService _denunciaService;

        public AdminController(IAdminService adminService, IDenunciaService denunciaService)
        {
            _adminService = adminService;
            _denunciaService = denunciaService;
        }

        /// <summary>
        /// Obtém as estatísticas do dashboard administrativo (total de utilizadores, publicações, bazes, denúncias, contas ativas, perfis privados).
        /// </summary>
        /// <returns>Objeto <see cref="AdminDashboardDto"/> com as estatísticas do sistema.</returns>
        /// <response code="200">Dashboard com as estatísticas do sistema.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(AdminDashboardDto), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ObterDashboard()
        {
            var dashboard = await _adminService.ObterDashboardAsync();
            return Ok(dashboard);
        }

        /// <summary>
        /// Lista todos os utilizadores registados (incluindo administradores).
        /// </summary>
        /// <returns>Lista de <see cref="UtilizadorDto"/> com todos os utilizadores.</returns>
        /// <response code="200">Lista de utilizadores devolvida com sucesso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet("utilizadores")]
        [ProducesResponseType(typeof(List<UtilizadorDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarUtilizadores()
        {
            var utilizadores = await _adminService.ListarUtilizadoresAsync();
            return Ok(utilizadores);
        }

        /// <summary>
        /// Lista todas as publicações ativas do sistema.
        /// </summary>
        /// <returns>Lista de <see cref="PublicacaoFeedDto"/> com todas as publicações.</returns>
        /// <response code="200">Lista de publicações devolvida com sucesso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet("publicacoes")]
        [ProducesResponseType(typeof(List<PublicacaoFeedDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarPublicacoes()
        {
            var publicacoes = await _adminService.ListarPublicacoesAsync();
            return Ok(publicacoes);
        }

        /// <summary>
        /// Lista todas as denúncias registadas, ordenadas da mais recente para a mais antiga.
        /// </summary>
        /// <returns>Lista de <see cref="DenunciaDto"/> com todas as denúncias.</returns>
        /// <response code="200">Lista de denúncias devolvida com sucesso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet("denuncias")]
        [ProducesResponseType(typeof(List<DenunciaDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarDenuncias()
        {
            var denuncias = await _adminService.ListarDenunciasAsync();
            return Ok(denuncias);
        }

        /// <summary>
        /// Cria um novo utilizador com nível de acesso Admin.
        /// </summary>
        /// <param name="dto">Dados do novo utilizador admin</param>
        /// <returns>Dados do utilizador criado</returns>
        /// <response code="201">Utilizador admin criado com sucesso</response>
        /// <response code="400">E-mail ou nome de utilizador já em uso, ou dados inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("utilizadores")]
        [ProducesResponseType(typeof(UtilizadorDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CriarUtilizador([FromBody] CriarUtilizadorAdminRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var resultado = await _adminService.CriarUtilizadorAsync(dto);

            if (resultado == null)
                return BadRequest("E-mail ou nome de utilizador já em uso.");

            return CreatedAtAction(nameof(ListarUtilizadores), resultado);
        }

        /// <summary>
        /// Atualiza o estado de uma denúncia (Pendente, Resolvida, Ignorada). Ao resolver, permite remover o conteúdo denunciado.
        /// </summary>
        /// <param name="id">ID da denúncia</param>
        /// <param name="estadoNovo">Novo estado: 0 = Pendente, 1 = Resolvida, 2 = Ignorada</param>
        /// <returns>Denúncia atualizada</returns>
        /// <response code="200">Estado da denúncia atualizado com sucesso</response>
        /// <response code="400">Estado inválido</response>
        /// <response code="404">Denúncia não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("denuncias/{id:int}/estado")]
        [ProducesResponseType(typeof(DenunciaDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AtualizarEstadoDenuncia(
            [FromRoute] int id,
            [FromBody] AtualizarEstadoDenunciaRequestDto estadoNovo)
        {
            if (!Enum.IsDefined(typeof(EstadoDenuncia), estadoNovo.EstadoDenuncia))
                return BadRequest("Estado inválido. Valores permitidos: 0 (Pendente), 1 (Resolvida), 2 (Ignorada).");

            var denuncia = await _denunciaService.AtualizarEstadoDenunciaAsync(id, estadoNovo.EstadoDenuncia);
            if (denuncia == null) return NotFound("Denúncia não encontrada.");

            return Ok(denuncia);
        }
    }
}
