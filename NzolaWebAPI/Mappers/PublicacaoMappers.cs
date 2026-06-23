using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Comentario;
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
                Texto = modelPublicacao.Texto,
                Ficheiros =
                    modelPublicacao.Ficheiros != null
                        ? modelPublicacao
                            .Ficheiros.Select(f => new FicheiroPublicacaoDto
                            {
                                Id = f.Id,
                                CaminhoFicheiro = f.CaminhoFicheiro,
                                TipoMime = f.TipoMime,
                                TamanhoBytes = f.TamanhoBytes,
                                DataUpload = f.DataUpload,
                            })
                            .ToList()
                        : new List<FicheiroPublicacaoDto>(),
                Comentarios =
                    modelPublicacao.Comentarios.Select(cm => cm.ToComentarioDto()).ToList()
                    ?? new List<ComentarioDto>(),
            };
        }

        public static PublicacaoFeedDto ToPublicacaoFeedDto(this Publicacao modelPublicacao)
        {
            if (modelPublicacao == null)
                return null;

            return new PublicacaoFeedDto
            {
                Id = modelPublicacao.Id,

                QuantidadeBazes = modelPublicacao.QuantidadeBazes,
                QuantidadeComentarios = modelPublicacao.QuantidadeComentarios,
                DataPublicacao = modelPublicacao.DataPublicacao,

                Autor =
                    modelPublicacao.Utilizador != null
                        ? modelPublicacao.Utilizador.ToAutorPublicacaoDto()
                        : null,
                Texto = modelPublicacao.Texto,
                Ficheiros =
                    modelPublicacao.Ficheiros != null
                        ? modelPublicacao
                            .Ficheiros.Select(f => new FicheiroPublicacaoDto
                            {
                                Id = f.Id,
                                CaminhoFicheiro = f.CaminhoFicheiro,
                                TipoMime = f.TipoMime,
                                TamanhoBytes = f.TamanhoBytes,
                                DataUpload = f.DataUpload,
                            })
                            .ToList()
                        : new List<FicheiroPublicacaoDto>(),
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
                Texto = publicacaoDto.Texto,
                Ficheiros = new List<FicheiroConteudo>(),
                Comentarios = new List<Comentario>(),
            };
        }
    }
}
