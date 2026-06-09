using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Repositories;
using NzolaWebAPI.Services;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConteudosPublicacaoController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly IPublicacaoRepository _pubRepo;
        private readonly IConteudoPublicacaoService _service;

        public ConteudosPublicacaoController(
            ContextoBDNzola contexto,
            IPublicacaoRepository pubRepo,
            IConteudoPublicacaoService service
        )
        {
            _contexto = contexto;
            _pubRepo = pubRepo;
            _service = service;
        }

        [HttpGet("publicacao/{publicacaoId}")]
        public async Task<IActionResult> SelecionarConteudoPublicacoes([FromRoute] int publicacaoId)
        {
            var conteudoPublicacoes = await _contexto
                .ConteudosPublicacao.Where(c => c.PublicacaoId == publicacaoId)
                .Select(cp => cp.ToConteudoPublicacaoDto())
                .ToListAsync();
            return Ok(conteudoPublicacoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> SelecionarConteudoPublicacao([FromRoute] int id)
        {
            var conteudo = await _contexto.ConteudosPublicacao.FindAsync(id);

            if (conteudo == null)
            {
                return NotFound();
            }

            return Ok(conteudo.ToConteudoPublicacaoDto());
        }

        [HttpPost("add-conteudo/{publicacaoId}")]
        public async Task<IActionResult> AdicionarConteudo(
            [FromForm] List<ItemConteudoRequestDto> conteudosPublicacaoDto,
            [FromRoute] int publicacaoId
        )
        {
            // 1. Validação rápida de entrada
            if (conteudosPublicacaoDto == null || !conteudosPublicacaoDto.Any())
            {
                return BadRequest("Forneça pelo menos um conteúdo para adicionar à publicação.");
            }
            // 2. Validação rápida de integridade
            bool publicacaoExiste = await _pubRepo.ExisteAsync(publicacaoId); // Via Repositório
            if (!publicacaoExiste)
                return BadRequest("Esta publicação não existe.");

            try
            {
                // 3. Executa o fluxo dinâmico através do Service
                List<ConteudoPublicacaoDto> blocosGravadosDtos = await _service.AdicionarListaAsync(
                    conteudosPublicacaoDto,
                    publicacaoId
                );

                // Retorna 200 Ok com a lista de todos os blocos adicionados com sucesso
                return Ok(
                    new
                    {
                        Mensagem = $"{blocosGravadosDtos.Count} novo(s) bloco(s) adicionado(s) com sucesso!",
                        NovosBlocos = blocosGravadosDtos,
                    }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    $"Erro interno ao tentar salvar os novos blocos: {ex.Message}"
                );
            }
        }

        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> ActualizarConteudoPublicacao(
            [FromRoute] int Id,
            [FromBody] ActualizarConteudoPublicacaoRequestDto conteudoPublicacaoDto
        )
        {
            var conteudoPublicacao = await _contexto.ConteudosPublicacao.FirstOrDefaultAsync(cp =>
                cp.Id == Id
            );

            if (conteudoPublicacao == null)
            {
                return NotFound();
            }

            conteudoPublicacao.Conteudo = conteudoPublicacaoDto.Conteudo;
            conteudoPublicacao.TipoConteudo = conteudoPublicacaoDto.TipoConteudo;
            conteudoPublicacao.Ordem = conteudoPublicacaoDto.Ordem;

            await _contexto.SaveChangesAsync();
            return Ok(conteudoPublicacao.ToConteudoPublicacaoDto());
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> EliminarConteudoPublicacao([FromRoute] int Id)
        {
            var conteudoPublicacao = await _contexto.ConteudosPublicacao.FirstOrDefaultAsync(cp =>
                cp.Id == Id
            );

            if (conteudoPublicacao == null)
            {
                return NotFound();
            }

            _contexto.ConteudosPublicacao.Remove(conteudoPublicacao);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }
    }
}
