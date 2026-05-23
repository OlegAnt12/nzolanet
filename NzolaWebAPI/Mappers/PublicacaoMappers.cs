using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models;
using NzolaWebAPI.DTOs.Publicacao;

namespace NzolaWebAPI.Mappers
{
    public class PublicacaoMappers
    {
        public static PublicacaoDto ToPublicacaoDto(this Publicacao modelPublicacao)
        {
            return new PublicacaoDto
            {
                Id = modelPublicacao.Id,
                AutorId = modelPublicacao,
                QuantidadeBazes = modelPublicacao,
                QuantidadeComentarios = modelPublicacao,
                DataPublicacao = modelPublicacao
            };
        }

        public static Publicacao ParaPublicacaoDePublicacaoDto(int autorId)
        {
            return new Publicacao
            {
                AutorId = autorId,
                QuantidadeBazes = 0,
                QuantidadeComentarios = 0
            };
        }
    }
}