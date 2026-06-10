using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Comentario;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Mappers
{
    public static class ComentarioMappers
    {
        public static ComentarioDto ToComentarioDto(this Comentario comentario)
        {
            return new ComentarioDto
            {
                Id = comentario.Id,
                PublicacaoId = comentario.PublicacaoId,
                ComentadorId = comentario.ComentadorId,
                ConteudoComentario = comentario.ConteudoComentario,
                DataComentario = comentario.DataComentario,
                DataActualizacao = comentario.DataActualizacao,
            };
        }

        public static Comentario ParaComentarioDeComentarioDto(this AdicionarComentarioRequestDto comentarioDto, int publicacaoId, int utilizadorId)
        {
            return new Comentario
            {
                PublicacaoId = publicacaoId,
                ComentadorId = utilizadorId,
                ConteudoComentario = comentarioDto.ConteudoComentario
            };
        }
    }
}
