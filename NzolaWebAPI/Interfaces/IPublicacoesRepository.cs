using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IPublicacaoRepository
    {
        Task<bool> ExisteAsync(int id);
        Task AdicionarAsync(Publicacao publicacao);
        Task<bool> SalvarAsync();
        Task<List<Publicacao>> ListarRecentesAsync();
        Task<Publicacao?> SelecionarAsync(int id);

        Task ExecutarEmEstrategiaAsync(Func<Task> acao);
        Task IniciarTransacaoAsync();
        Task ConfirmarTransacaoAsync();
        Task CancelarTransacaoAsync();
    }
}
