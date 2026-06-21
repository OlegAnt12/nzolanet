using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Seguidor;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Mappers
{
    public static class SeguidorMappers
    {
        public static SeguidorDto ToSeguidorDto(this Seguidor modelSeguidor)
        {
            return new SeguidorDto
            {
                Id = modelSeguidor.Id,
                SeguidorId = modelSeguidor.SeguidorId,
                SeguidoId = modelSeguidor.SeguidoId,
                DataInicio = modelSeguidor.DataInicio,
            };
        }

        public static SeguidorFeedDto ToSeguidorFeedDto(this Seguidor modelSeguidor)
        {
            return new SeguidorFeedDto
            {
                Id = modelSeguidor.Id,
                Seguidor = modelSeguidor.UtilizadorSeguidor != null
            ? modelSeguidor.UtilizadorSeguidor.ToAutorPublicacaoDto()
            : null,
                Seguido = modelSeguidor.UtilizadorSeguido != null
            ? modelSeguidor.UtilizadorSeguido.ToAutorPublicacaoDto()
            : null,
                DataInicio = modelSeguidor.DataInicio,
            };
        }

        public static Seguidor ToSeguidorFromCriarDto(
            this CriarSeguidorDto criarSeguidorDto,
            int seguidorId,
            int seguidoId
        )
        {
            return new Seguidor { SeguidorId = seguidorId, SeguidoId = seguidoId };
        }
    }
}
