using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Publicacao;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;
using NzolaWebAPI.Repositories;
using NzolaWebAPI.Services;

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
        public async Task<IActionResult> ListarPublicacoes()
        {
            var publicacoes = await _pubRepo.ListarRecentesAsync();
            var publicacaoesDtos = publicacoes.Select(p => p.ToPublicacaoDto()).ToList();
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
                || publicacaoDto.Conteudos == null
                || !publicacaoDto.Conteudos.Any()
            )
            {
                return BadRequest(
                    "A publicação necessita de pelo menos um bloco de conteúdo (Texto, Imagem ou Vídeo)."
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

            /*var strategy = _contexto.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _contexto.Database.BeginTransactionAsync();

                try
                {
                    var publicacao = publicacaoDto.ParaPublicacaoDePublicacaoDto(utilizadorId);
                    publicacao.DataPublicacao = DateTime.Now;

                    await _contexto.Publicacoes.AddAsync(publicacao);
                    await _contexto.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return CreatedAtAction(
                        nameof(SelecionarPublicacao),
                        new { id = publicacao.Id },
                        publicacao.ToPublicacaoDto()
                    );
                }
                catch (Exception exc)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(
                        500,
                        $"Erro Interno ao tentar registar a publicação: {exc.Message}"
                    );
                }
            });*/
        }

        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> ActualizarPublicacao(
            [FromRoute] int Id,
            [FromBody] ActualizarPublicacaoRequestDto putPublicacaoDto
        )
        {
            var publicacao = await _contexto.Publicacoes.FirstOrDefaultAsync(p => p.Id == Id);

            if (publicacao == null)
            {
                return NotFound();
            }

            publicacao.DataAtualizacaoPublicacao = DateTime.Now;

            await _contexto.SaveChangesAsync();

            return Ok(publicacao.ToPublicacaoDto());
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> EliminarPublicacao([FromRoute] int Id)
        {
            var publicacao = await _contexto.Publicacoes.FirstOrDefaultAsync(p => p.Id == Id);

            if (publicacao == null)
            {
                return NotFound();
            }

            _contexto.Publicacoes.Remove(publicacao);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }
    }
}
