using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.DTOs.Denuncia;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DenunciasController : ControllerBase
    {
        private readonly IDenunciaService _denunciaService;

        public DenunciasController(IDenunciaService denunciaService)
        {
            _denunciaService = denunciaService;
        }

        [HttpPost]
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

        [HttpGet]
        public async Task<IActionResult> ListarDenuncias()
        {
            var denuncias = await _denunciaService.ListarTodasAsync();
            return Ok(denuncias);
        }

        [HttpGet("entidade/{tipoEntidade}/{idEntidade}")]
        public async Task<IActionResult> ListarDenunciasPorEntidade(TipoEntidade tipoEntidade, int idEntidade)
        {
            var denuncias = await _denunciaService.ListarPorEntidadeAsync(tipoEntidade, idEntidade);
            return Ok(denuncias);
        }
    }
}
