using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Interfaces
{
    public interface IDenunciaRepository
    {
        Task AdicionarAsync(Denuncia denuncia);
        Task<List<Denuncia>> ListarTodasAsync();
        Task<List<Denuncia>> ListarPorEntidadeAsync(TipoEntidade tipoEntidade, int idEntidade);
        Task<bool> SalvarAsync();
    }
}
