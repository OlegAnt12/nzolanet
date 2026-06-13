using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Seguidor;

namespace NzolaWebAPI.Interfaces
{
    public interface ISeguidorService
    {
        Task<SeguirResultadoDto> AlternarSeguirAsync(int seguidorId, int seguidoId);
    }
}