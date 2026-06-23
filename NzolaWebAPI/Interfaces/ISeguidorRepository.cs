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
        Task<Seguidor> SelecionarRelacaoIdAsync(int Id);

        Task<List<Seguidor>> ListarSeguidoresAsync(int utilizadorId);
        Task<List<Seguidor>> ListarSeguindoAsync(int utilizadorId);

        Task<int> ContarSeguidoresAsync(int utilizadorId);
        Task<int> ContarSeguindoAsync(int utilizadorId);

        Task AdicionarAsync(Seguidor seguidor);
        void Remover(Seguidor seguidor);
        Task<bool> SalvarAsync();
    }
}
