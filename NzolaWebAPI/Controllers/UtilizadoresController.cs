using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs;
using NzolaWebAPI.DTOs.Utilizador;  
using NzolaWebAPI.Models;     
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Mappers;  




namespace NzolaWebAPI.Controllers{
[Route("api/[Controller]")]
[ApiController]
public class UtilizadoresController : ControllerBase{
    private readonly ContextoBDNzola _contexto;
    public UtilizadoresController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }
       
       [HttpGet]
         public async Task<IActionResult> ListarUtilizadores()
         {
              var utilizadores = await _contexto.Utilizadores.Select(u => u.ToUtilizadorDto())
              .ToListAsync();
              return Ok(utilizadores);
         }

        [HttpGet("{id}")]

    public async Task<IActionResult> SelecionarUtilizador(int id)
        {
            var utilizador = await _contexto.Utilizadores.FindAsync(id);

            if (utilizador == null)
            {
                return NotFound();
            }

            return Ok(utilizador.ToUtilizadorDto());
        }
        
       [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarUtilizadorRequestDto criarUtilizadorDto)
        {
            var utilizador = criarUtilizadorDto.ToUtilizadorFromCriarDto();

            _contexto.Utilizadores.Add(utilizador);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(SelecionarUtilizador), new { id = utilizador.Id }, utilizador.ToUtilizadorDto());                                             
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

        

}

}

