using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Baze;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Mappers
{
    public static class BazeMappers
    {
        public static BazeDto ToBazeDto(this Baze bazeModel)
        {
            return new BazeDto
            {
                Id = bazeModel.Id,
                PublicacaoId = bazeModel.PublicacaoId,
                UtilizadorId = bazeModel.UtilizadorId,
                DataInteracao = bazeModel.DataInteracao
            };
        }
    }
}