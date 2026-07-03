using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Publicacao;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicacoesController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly IPublicacaoRepository _pubRepo;
        private readonly IPublicacaoService _publicacaoService;

        public PublicacoesController(
            ContextoBDNzola contexto,
            IPublicacaoRepository pubRepo,
            IPublicacaoService publicacaoService
        )
        {
            _contexto = contexto;
            _pubRepo = pubRepo;
            _publicacaoService = publicacaoService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPublicacoes([FromQuery] int? utilizadorLogadoId = null, [FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
        {
            var (publicacoes, total) = await _publicacaoService.ListarFeedAsync(utilizadorLogadoId, pagina, tamanho);
            return Ok(new { publicacoes, total, pagina, tamanho });
        }

        [HttpGet("todas")]
        public async Task<IActionResult> ListarTodasPublicacoes()
        {
            var publicacoes = await _pubRepo.ListarRecentesAsync();
            var publicacaoesDtos = publicacoes.Select(p => p.ToPublicacaoFeedDto()).ToList();
            return Ok(publicacaoesDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> SelecionarPublicacao(int id)
        {
            var publicacao = await _pubRepo.SelecionarAsync(id);

            if (publicacao == null)
            {
                return NotFound();
            }

            return Ok(publicacao.ToPublicacaoFeedDto());
        }

        [HttpPost("{utilizadorId}")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(209715200)]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> PublicarConteudo(
            [FromRoute] int utilizadorId,
            [FromForm] CriarPublicacaoRequestDto publicacaoDto
        )
        {
            bool utilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Id == utilizadorId
            );
            if (!utilizadorExiste)
            {
                return BadRequest("Este Utilizador não existente");
            }

            if (
                publicacaoDto == null
                || (
                    string.IsNullOrWhiteSpace(publicacaoDto.Texto)
                    && (publicacaoDto.Ficheiros == null || !publicacaoDto.Ficheiros.Any())
                )
            )
            {
                return BadRequest(
                    "A publicação necessita de um conteúdo válido (Texto ou pelo menos um ficheiro)."
                );
            }

            try
            {
                // 2. Delegação total à camada de Serviço
                var resultadoDto = await _publicacaoService.CriarAsync(utilizadorId, publicacaoDto);

                if (resultadoDto == null)
                {
                    return BadRequest("Não foi possível registar a publicação de momento.");
                }

                // 3. Resposta Padrão REST (201 Created com o DTO completo mapeado)
                return CreatedAtAction(
                    "SelecionarPublicacao", // Nome do método HTTP GET por ID
                    new { id = resultadoDto.Id },
                    resultadoDto
                );
            }
            catch (Exception exc)
            {
                // Captura qualquer erro lançado pelo Rollback do Service e protege o servidor
                return StatusCode(
                    500,
                    $"Erro Interno ao tentar processar e registar a publicação: {exc.Message}"
                );
            }
        }

        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> ActualizarPublicacao(
            [FromRoute] int Id,
            [FromBody] ActualizarPublicacaoRequestDto putPublicacaoDto
        )
        {
            var publicacao = await _publicacaoService.ActualizarAsync(Id, putPublicacaoDto);

            if (publicacao == null)
            {
                return NotFound();
            }

            await _pubRepo.SalvarAsync();

            return Ok(publicacao.ToPublicacaoDto());
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> EliminarPublicacao([FromRoute] int Id)
        {
            var publicacao = await _publicacaoService.EliminarAsync(Id);

            if (publicacao == null)
            {
                return NotFound();
            }

            await _pubRepo.SalvarAsync();

            return NoContent();
        }
    }
}
