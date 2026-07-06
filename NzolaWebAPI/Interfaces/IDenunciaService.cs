using NzolaWebAPI.DTOs.Denuncia;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Interfaces
{
    public interface IDenunciaService
    {
        Task<DenunciaDto?> CriarDenunciaAsync(CriarDenunciaDto dto);
        Task<List<DenunciaDto>> ListarTodasAsync();
        Task<List<DenunciaDto>> ListarPorEntidadeAsync(TipoEntidade tipoEntidade, int idEntidade);
        Task<DenunciaDto?> AtualizarEstadoDenunciaAsync(int id, EstadoDenuncia novoEstado);
    }
}
