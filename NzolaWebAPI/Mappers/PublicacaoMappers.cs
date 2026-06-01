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
                Conteudos = modelPublicacao
                    .Conteudos.Select(ct => ct.ToConteudoPublicacaoDto())
                    .ToList(),
                Comentarios = modelPublicacao
                    .Comentarios.Select(cm => cm.ToComentarioDto())
                    .ToList(),
            };
        }

        public static Publicacao ParaPublicacaoDePublicacaoDto (
            this CriarPublicacaoRequestDto publicacaoDto,
            int autorId
        )
        {
            var publicacao = new Publicacao
            {
                AutorId = autorId,
                QuantidadeBazes = 0,
                QuantidadeComentarios = 0,
                Conteudos = new List<ConteudoPublicacao>(),
                Comentarios = new List<Comentario>(),
            };

            if (publicacaoDto.Conteudos != null)
            {
                foreach (var conteudoPubDto in publicacaoDto.Conteudos)
                {
                    var conteudoPublicacao = conteudoPubDto.ParaConteudoPublicacaoDeConteudoPublicacaoDto(publicacao.Id);
                    publicacao.Conteudos.Add(conteudoPublicacao);
                }
            }

            return publicacao;
        }
    }
}
