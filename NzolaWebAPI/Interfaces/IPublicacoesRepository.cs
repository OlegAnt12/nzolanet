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

        Task<int> ContarPorUtilizadorAsync(int utilizadorId);
        Task<int> ContarTodasAsync();
        Task AdicionarAsync(Publicacao publicacao);
        Task<bool> SalvarAsync();
        Task<List<Publicacao>> ListarRecentesPorFeedAsync(int? utilizadorLogadoId = null, int pagina = 1, int tamanho = 10);
        Task<int> ContarFeedAsync(int? utilizadorLogadoId = null);
        Task<List<Publicacao>> ListarRecentesAsync();
        Task<Publicacao?> SelecionarAsync(int id);

        Task ExecutarEmEstrategiaAsync(Func<Task> acao);
        Task IniciarTransacaoAsync();
        Task ConfirmarTransacaoAsync();
        Task CancelarTransacaoAsync();
    }
}
