using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Baze;

namespace NzolaWebAPI.Interfaces
{
    public interface IBazeService
    {
        Task<BazeResultadoDto> AlternarBazeAsync(int publicacaoId, int utilizadorId);
    }
}
