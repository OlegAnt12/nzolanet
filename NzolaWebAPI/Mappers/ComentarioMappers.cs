using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                UtilizadorId = comentario.UtilizadorId,
                ConteudoComentario = comentario.ConteudoComentario,
                DataComentario = comentario.DataComentario,
                DataActualizacao = comentario.DataActualizacao,
            };
        }
    }
}
