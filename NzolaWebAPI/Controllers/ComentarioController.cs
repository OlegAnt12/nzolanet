using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentarioController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public ComentarioController (ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult GetComentarios()
        {
            var comentarios =  _contexto.Comentarios.ToList();
            return Ok(comentarios);
        }

        [HttpGet("{id}")]
        public IActionResult GetComentario([FromRoute] int id)
        {
            var comentario =  _contexto.Comentarios.Find(id);

            if(comentario == null)
            {
                return NotFound();
            }
            
            return Ok(comentario);
        }
    }
}