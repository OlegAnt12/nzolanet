using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Publicacao;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Mappers
{
    public static class PublicacaoMappers
    {
        public static PublicacaoDto ToPublicacaoDto(this Publicacao modelPublicacao)
        {
            return new PublicacaoDto
            {
                Id = modelPublicacao.Id,
                AutorId = modelPublicacao.AutorId,
                QuantidadeBazes = modelPublicacao.QuantidadeBazes,
                QuantidadeComentarios = modelPublicacao.QuantidadeComentarios,
                DataPublicacao = modelPublicacao.DataPublicacao,
                Conteudos =
                    modelPublicacao.Conteudos.Select(ct => ct.ToConteudoPublicacaoDto()).ToList()
                    ?? new List<ConteudoPublicacaoDto>(),
                Comentarios =
                    modelPublicacao.Comentarios.Select(cm => cm.ToComentarioDto()).ToList()
                    ?? new List<ComentarioDto>(),
            };
        }

        public static PublicacaoDto ToPublicacaoFeedDto(this Publicacao modelPublicacao)
        {
            if (modelPublicacao == null)
                return null;

            return new PublicacaoDto
            {
                Id = modelPublicacao.Id,

                QuantidadeBazes = modelPublicacao.QuantidadeBazes,
                QuantidadeComentarios = modelPublicacao.QuantidadeComentarios,
                DataPublicacao = modelPublicacao.DataPublicacao,

                AutorId =
                    modelPublicacao.Utilizador != null
                        ? AutorId = modelPublicacao.Utilizador.ToUtilizadorDto()
                        : null,
                Conteudos =
                    modelPublicacao
                        .Conteudos?.OrderBy(ct => ct.Ordem)
                        .Select(ct => ct.ToConteudoPublicacaoDto())
                        .ToList()
                    ?? new List<ConteudoPublicacaoDto>(),
                Comentarios =
                    modelPublicacao
                        .Comentarios?.OrderBy(cm => cm.DataComentario)
                        .Select(cm => cm.ToComentarioDto())
                        .ToList()
                    ?? new List<ComentarioDto>(),
            };
        }

        public static Publicacao ParaPublicacaoDePublicacaoDto(
            this CriarPublicacaoRequestDto publicacaoDto,
            int autorId
        )
        {
            return new Publicacao
            {
                AutorId = autorId,
                QuantidadeBazes = 0,
                QuantidadeComentarios = 0,
                DataPublicacao = DateTime.Now,
                Conteudos = new List<ConteudoPublicacao>(),
                Comentarios = new List<Comentario>(),
            };
        }
    }
}
