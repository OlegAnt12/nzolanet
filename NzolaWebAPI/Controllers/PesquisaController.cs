using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.Mappers;

namespace NzolaWebAPI.Controllers
{
    /// <summary>
    /// Controlador para pesquisa global de utilizadores e publicações.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PesquisaController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public PesquisaController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        /// <summary>
        /// Pesquisa utilizadores e publicações com base num termo de pesquisa. Procura por nome, username, email e texto de publicações.
        /// </summary>
        /// <param name="termo">Termo de pesquisa</param>
        /// <param name="tipo">Tipo de pesquisa (tudo, publicacoes, perfis)</param>
        /// <returns>Lista de publicações e perfis correspondentes ao termo de pesquisa</returns>
        /// <response code="200">Resultados da pesquisa</response>
        /// <response code="400">Termo de pesquisa inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Pesquisar([FromQuery] string termo, [FromQuery] string tipo = "tudo")
        {
            if (string.IsNullOrWhiteSpace(termo) || termo.Length < 2)
                return Ok(new { publicacoes = new List<object>(), perfis = new List<object>() });

            object publicacoesResult = new List<object>();
            object perfisResult = new List<object>();

            if (tipo == "tudo" || tipo == "publicacoes")
            {
                var listaPub = await _contexto.Publicacoes
                    .Where(p => p.Existencia == Models.Enums.EstadoExistenciaLogica.Existente &&
                                p.Utilizador.Privacidade == Models.Enums.EstadoAcesso.Publico &&
                                p.Texto.Contains(termo))
                    .Include(p => p.Utilizador)
                    .Include(p => p.Ficheiros)
                    .Include(p => p.Comentarios).ThenInclude(c => c.Utilizador)
                    .OrderByDescending(p => p.DataPublicacao)
                    .Take(20)
                    .ToListAsync();

                publicacoesResult = listaPub.Select(p => p.ToPublicacaoFeedDto()).ToList();
            }

            if (tipo == "tudo" || tipo == "perfis")
            {
                var listaUtil = await _contexto.Utilizadores
                    .Where(u => u.EstadoConta == Models.Enums.EstadoConta.Activa &&
                                (u.NomeCompleto.Contains(termo) || u.NomeUtilizador.Contains(termo)))
                    .OrderBy(u => u.NomeCompleto)
                    .Take(20)
                    .ToListAsync();

                perfisResult = listaUtil.Select(u => new
                {
                    u.Id,
                    u.NomeCompleto,
                    u.NomeUtilizador,
                    FotoPerfil = u.FotoPerfil != null ? Convert.ToBase64String(u.FotoPerfil) : null,
                    u.Privacidade,
                    u.Biografia
                }).ToList();
            }

            return Ok(new { publicacoes = publicacoesResult, perfis = perfisResult });
        }
    }
}
