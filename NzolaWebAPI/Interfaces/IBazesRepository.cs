using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IBazesRepository
    {
        Task<List<Baze>> GetBazesPorPublicacaoAsync();
    }
}
