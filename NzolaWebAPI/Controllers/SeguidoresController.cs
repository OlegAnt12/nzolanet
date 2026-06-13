using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Baze;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Repositories;
using NzolaWebAPI.Services;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguidoresController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly ISeguidorRepository _seguidorRepo;
        private readonly ISeguidorService _seguidorService;

        public SeguidoresController(
            ContextoBDNzola contexto,
            ISeguidorRepository seguidorRepo,
            ISeguidorService seguidorService
        )
        {
            _contexto = contexto;
            _seguidorRepo = seguidorRepo;
            _seguidorService = seguidorService;
        }

        [HttpGet("utilizador/{utillizadorId}")]
        public async Task<IActionResult> ListarSeguidoresPorUtilizador(
            [FromRoute] int utillizadorId
        )
        {
            var seguidores = await _seguidorRepo.ListarSeguidoresPorUtilizadorAsync(utillizadorId);
            return Ok(seguidores);
        }

        [HttpGet("{relacaoId}")]
        public async Task<IActionResult> SelecionarSeguidor([FromRoute] int relacaoId)
        {
            var seguidor = await _seguidorRepo.SelecionarRelacaoIdAsync(relacaoId);

            if (seguidor == null)
            {
                return NotFound();
            }

            return Ok(seguidor.ToSeguidorDto());
        }

        [HttpPost]
        public async Task<IActionResult> AlternarSeguir(
            [FromRoute] int seguidorId,
            [FromRoute] int seguidoId
        )
        {
            var seguidor = await _seguidorService.AlternarSeguirAsync(seguidorId, seguidoId);
            if (seguidor.ErroMensagem != null)
            {
                return BadRequest(seguidor.ErroMensagem);
            }

            if (seguidor.FoiRemovido)
            {
                return Ok(new { mensagem = "Relação removida com sucesso!" });
            }
            return CreatedAtAction(
                nameof(SelecionarSeguidor),
                new { id = seguidor.SeguidorDto!.Id },
                seguidor.SeguidorDto
            );
        }
    }
}
