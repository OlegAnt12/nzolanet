using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Seguidor;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;


namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguidorController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto; 
        private readonly ISeguidorRepository _seguidorRepo;
        private readonly ISeguidorService _seguidorService;

        public SeguidorController(ContextoBDNzola contexto, ISeguidorRepository seguidorRepo, ISeguidorService seguidorService)
        {
            _contexto = contexto;
            _seguidorRepo = seguidorRepo;
            _seguidorService = seguidorService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ListarSeguidoresPorUtilizador([FromRoute] int id)
        {
            var seguidores = await _seguidorRepo.ListarSeguidoresPorUtilizadorAsync(id);
            return Ok(seguidores);
        }

        [HttpGet("{id}")]
        public IActionResult SelecionarSeguidor([FromRoute] int id)
        {
            var seguidor = _seguidorRepo.SelecionarRelacaoIdAsync(id);

            if (seguidor == null)
            {
                return NotFound();
            }
            return Ok(seguidor.ToSeguidorDto());
        }

        [HttpPost]
        public async Task<IActionResult> AlternarSeguir ([FromRoute] int seguidorId, [FromRoute] int seguidoId)
        {
            var seguidor = await _seguidorService.AlternarSeguirAsync(int seguidorId, int seguidoId);
            if (resultado.ErroMensagem != null)
            {
                return BadRequest(resultado.ErroMensagem);
            }

            if (resultado.FoiRemovido)
            {
                return Ok(
                    new
                    {
                        mensagem = "Relação removida com sucesso!",
                    }
                );
            }
            return CreatedAtAction(nameof(SelecionarSeguidor), new {id = seguidor.Id}, seguidor.ToSeguidorDto());
        }        
    }
}