using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface ISeguidorRepository
    {
        Task<List<Seguidor>> ListarSeguidoresPorUtilizadorAsync(int id);

        Task<Seguidor?> ObterPorRelacaoAsync(int seguidorId, int seguidoId);
        Task<Seguidor?> SelecionarRelacaoIdAsync(int Id);
        Task AdicionarAsync(Seguidor seguidor);

        Task<bool> SalvarAsync();
    }
}