using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IBazeRepository
    {
        Task<List<Baze>> GetBazesPorPublicacaoAsync(int id);
        Task<Baze> SelecionarBazeAsync(int id);

        Task<Baze?> ObterPorPublicacaoEUtilizadorAsync(int publicacaoId, int utilizadorId);
        Task AdicionarAsync(Baze baze);
        void Remover(Baze baze);
        Task<bool> SalvarAsync();
    }
}
