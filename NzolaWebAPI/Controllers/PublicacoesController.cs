using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Publicacao;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicacoesController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public PublicacoesController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPublicacoes()
        {
            var publicacoes = await _contexto
                .Publicacoes.ToListAsync()
                .Select(p => p.ToPublicacaoDto());
            return Ok(publicacoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> SelecionarPublicacao(int id)
        {
            var publicacao = await _contexto.Publicacoes.FindAsync(id);

            if (publicacao == null)
            {
                return NotFound();
            }

            return Ok(publicacao.ToPublicacaoDto());
        }

        [HttpPost("{utilizadorId}")]
        public async Task<IActionResult> PublicarConteudo(
            [FromRoute] int utilizadorId,
            [FromBody] CriarPublicacaoRequestDto publicacaoDto
        )
        {
            bool utilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Id == utilizadorId
            );
            if (!utilizadorExiste)
            {
                return BadRequest("Este Utilizador não existente");
            }

            var strategy = _contexto.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _contexto.Database.BeginTransactionAsync();

                try
                {
                    var publicacao = publicacaoDto.ParaPublicacaoDePublicacaoDto(utilizadorId);
                    publicacao.DataPublicacao = DateTime.Now;

                    await _contexto.Publicacoes.AddAsync(publicacao);
                    await _contexto.Publicacoes.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return CreatedAtAction(
                        nameof(SelecionarPublicacao),
                        new { id = publicacao.Id },
                        publicacao.ToPublicacaoDto()
                    );
                }
                catch (Exception exc)
                {
                    await transactionRollbackAsync();
                    return StausCode(
                        500,
                        $"Erro Interno ao tentar registar a publicação: {exc.Message}"
                    );
                }
            });
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

            publicacao.QuantidadeBazes = putPublicacaoDto.QuantidadeBazes;
            publicacao.QuantidadeComentarios = putPublicacaoDto.QuantidadeComentarios;
            publicacao.DataAtualizacaoPublicacao = DateTime.Now;

            await contexto.SaveChangesAsync();

            return Ok(publicacao.ToPublicacaoDto());
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> EliminarPublicacao([FromRoute] int Id)
        {
            var publicacao = _contexto.Publicacoes.FirstOrDefaultAsync(p => p.Id == Id);

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
