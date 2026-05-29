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
            this AdicionarConteudoPublicacaoRequestDto conteudoDto
        )
        {
            return new ConteudoPublicacao
            {
                PublicacaoId = conteudoDto.PublicacaoId,
                Conteudo = conteudoDto.Conteudo,
                TipoConteudo = conteudoDto.TipoConteudo,
                Ordem = conteudoDto.Ordem,
            };
        }
    }
}
