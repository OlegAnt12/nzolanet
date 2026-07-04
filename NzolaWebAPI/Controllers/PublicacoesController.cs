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
    /// <summary>
    /// Controlador para gestão de publicações (CRUD e listagem).
    /// </summary>
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

        /// <summary>
        /// Lista as publicações do feed do utilizador autenticado (inclui publicações de quem o utilizador segue).
        /// </summary>
        /// <param name="utilizadorLogadoId">ID do utilizador autenticado</param>
        /// <param name="pagina">Número da página</param>
        /// <param name="tamanho">Quantidade de publicações por página</param>
        /// <returns>Lista paginada de publicações do feed</returns>
        /// <response code="200">Feed de publicações retornado com sucesso</response>
        /// <response code="400">Parâmetros de paginação inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarPublicacoes([FromQuery] int? utilizadorLogadoId = null, [FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
        {
            var (publicacoes, total) = await _publicacaoService.ListarFeedAsync(utilizadorLogadoId, pagina, tamanho);
            return Ok(new { publicacoes, total, pagina, tamanho });
        }

        /// <summary>
        /// Lista todas as publicações ativas do sistema, ordenadas da mais recente para a mais antiga.
        /// </summary>
        /// <returns>Lista de todas as publicações ativas</returns>
        /// <response code="200">Publicações retornadas com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("todas")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarTodasPublicacoes()
        {
            var publicacoes = await _pubRepo.ListarRecentesAsync();
            var publicacaoesDtos = publicacoes.Select(p => p.ToPublicacaoFeedDto()).ToList();
            return Ok(publicacaoesDtos);
        }

        /// <summary>
        /// Obtém os detalhes de uma publicação específica pelo seu ID.
        /// </summary>
        /// <param name="id">ID da publicação</param>
        /// <returns>Detalhes da publicação</returns>
        /// <response code="200">Publicação encontrada</response>
        /// <response code="404">Publicação não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SelecionarPublicacao(int id)
        {
            var publicacao = await _pubRepo.SelecionarAsync(id);

            if (publicacao == null)
            {
                return NotFound();
            }

            return Ok(publicacao.ToPublicacaoFeedDto());
        }

        /// <summary>
        /// Cria uma nova publicação com suporte para upload de ficheiros (imagens/vídeos).
        /// </summary>
        /// <param name="utilizadorId">ID do autor da publicação</param>
        /// <param name="publicacaoDto">Dados da publicação e ficheiros</param>
        /// <returns>Publicação criada</returns>
        /// <response code="201">Publicação criada com sucesso</response>
        /// <response code="400">Dados inválidos ou utilizador inexistente</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{utilizadorId}")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(209715200)]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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
                var resultadoDto = await _publicacaoService.CriarAsync(utilizadorId, publicacaoDto);

                if (resultadoDto == null)
                {
                    return BadRequest("Não foi possível registar a publicação de momento.");
                }

                return CreatedAtAction(
                    "SelecionarPublicacao",
                    new { id = resultadoDto.Id },
                    resultadoDto
                );
            }
            catch (Exception exc)
            {
                return StatusCode(
                    500,
                    $"Erro Interno ao tentar processar e registar a publicação: {exc.Message}"
                );
            }
        }

        /// <summary>
        /// Atualiza o texto de uma publicação existente (apenas o autor pode editar).
        /// </summary>
        /// <param name="Id">ID da publicação a editar</param>
        /// <param name="putPublicacaoDto">Novos dados da publicação</param>
        /// <returns>Publicação atualizada</returns>
        /// <response code="200">Publicação atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Publicação não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut]
        [Route("{Id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Remove uma publicação do sistema (apenas o autor pode eliminar).
        /// </summary>
        /// <param name="Id">ID da publicação a eliminar</param>
        /// <returns>Sem conteúdo</returns>
        /// <response code="204">Publicação eliminada com sucesso</response>
        /// <response code="404">Publicação não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete]
        [Route("{Id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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
