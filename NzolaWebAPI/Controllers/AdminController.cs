using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Dtos.Admin;
using NzolaWebAPI.DTOs.Denuncia;
using NzolaWebAPI.DTOs.Publicacao;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Interfaces;

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

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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
    }
}
