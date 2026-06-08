using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Mappers
{
    public static class ConteudoPublicacaoMappers
    {
        public static ConteudoPublicacaoDto ToConteudoPublicacaoDto(
            this ConteudoPublicacao conteudoPublicacaoModel
        )
        {
            return new ConteudoPublicacaoDto
            {
                Id = conteudoPublicacaoModel.Id,
                PublicacaoId = conteudoPublicacaoModel.PublicacaoId,
                Conteudo = conteudoPublicacaoModel.Conteudo,
                TipoConteudo = conteudoPublicacaoModel.TipoConteudo,
                Ordem = conteudoPublicacaoModel.Ordem,
            };
        }

        public static ConteudoPublicacao ParaConteudoPublicacaoDeConteudoPublicacaoDto(
            this AdicionarConteudoPublicacaoRequestDto conteudoDto,
            int publicacaoId
        )
        {
            return new ConteudoPublicacao
            {
                PublicacaoId = publicacaoId,
                Conteudo = conteudoDto.Conteudo,
                TipoConteudo = conteudoDto.TipoConteudo,
                Ordem = conteudoDto.Ordem,
            };
        }

        public static ConteudoPublicacao ParaConteudoPublicacaoDeItemConteudoRequestDto(
            this ItemConteudoRequestDto conteudoDto,
            int publicacaoId,
            string conteudoResolvido
        )
        {
            return new ConteudoPublicacao
            {
                PublicacaoId = publicacaoId,
                Conteudo = conteudoResolvido,
                TipoConteudo = conteudoDto.TipoConteudo,
                Ordem = conteudoDto.Ordem,
            };
        }
    }
}
