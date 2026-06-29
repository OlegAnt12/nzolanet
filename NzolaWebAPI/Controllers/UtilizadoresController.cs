using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UtilizadoresController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly IUtilizadorService _utilizadorService;

        private readonly ISeguidorRepository _seguidorRepository;
        private readonly IPublicacaoRepository _publicacaoRepository;

        public UtilizadoresController(
            ContextoBDNzola contexto,
            IUtilizadorService utilizadorService,
            ISeguidorRepository seguidorRepository,
            IPublicacaoRepository publicacaoRepository
        )
        {
            _contexto = contexto;
            _utilizadorService = utilizadorService;
            _seguidorRepository = seguidorRepository;
            _publicacaoRepository = publicacaoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ListarUtilizadores()
        {
            var utilizadores = await _contexto
                .Utilizadores.Where(u => u.NivelAcesso != (NivelAcesso)1)
                .Select(u => u.ToUtilizadorDto())
                .ToListAsync();
            return Ok(utilizadores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> SelecionarUtilizador(int id, [FromQuery] int? utilizadorLogadoId = null)
        {
            var utilizador = await _utilizadorService.ObterPorIdServiceAsync(id, utilizadorLogadoId);

            if (utilizador == null)
            {
                return NotFound();
            }

            return Ok(utilizador);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(
            [FromBody] CriarUtilizadorRequestDto criarUtilizadorDto
        )
        {
            var utilizador = criarUtilizadorDto.ToUtilizadorFromCriarDto();

            await _contexto.Utilizadores.AddAsync(utilizador);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(
                nameof(SelecionarUtilizador),
                new { id = utilizador.Id },
                utilizador.ToUtilizadorDto()
            );
        }

        [HttpPut("perfil/{id}")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(209715200)]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> AtualizarPerfil(
            int id,
            [FromForm] ActualizarPerfilRequestDto dto
        )
        {
            if (string.IsNullOrWhiteSpace(dto.NomeCompleto))
            {
                return BadRequest("O nome completo é obrigatório.");
            }

            var utilizadorAtualizado = await _utilizadorService.AtualizarPerfilAsync(id, dto);
            if (utilizadorAtualizado == null)
                return NotFound("Utilizador não encontrado.");

            return Ok(utilizadorAtualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Apagar(int id)
        {
            var utilizador = await _contexto.Utilizadores.FindAsync(id);

            if (utilizador == null)
            {
                return NotFound();
            }

            _contexto.Utilizadores.Remove(utilizador);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("estatisticas/{id:int}")]
        public async Task<IActionResult> ObterEstatisticas(int id)
        {
            var estatisticas = await _utilizadorService.ObterEstatisticasAsync(id);
            return Ok(estatisticas);
        }

        private int? ObterUtilizadorLogadoId()
        {
            // Implemente conforme sua autenticação (JWT, Session, etc)
            var userId = User.FindFirst("id")?.Value;
            return userId != null ? int.Parse(userId) : null;
        }
    }
}
