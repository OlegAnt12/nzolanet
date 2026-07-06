using System.Threading.Tasks;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IUtilizadorRepository
    {
        Task<Utilizador?> ObterPorEmailAsync(string email);
        Task<Utilizador?> ObterPorIdAsync(int id);
        Task<Utilizador?> ObterPorTokenRedefinirPasswordAsync(string token);

        Task AdicionarAsync(Utilizador utilizador);
        Task<bool> ExisteEmailAsync(string email);
        Task<bool> SalvarAsync();
        Task<int> ContarTodosAsync();
    }
}
