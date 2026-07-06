using System.Collections.Generic;
using System.Threading.Tasks;
using NzolaWebAPI.Dtos.Admin;
using NzolaWebAPI.DTOs.Denuncia;
using NzolaWebAPI.DTOs.Publicacao;
using NzolaWebAPI.DTOs.Utilizador;

namespace NzolaWebAPI.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> ObterDashboardAsync();
        Task<List<UtilizadorDto>> ListarUtilizadoresAsync();
        Task<List<PublicacaoFeedDto>> ListarPublicacoesAsync();
        Task<List<DenunciaDto>> ListarDenunciasAsync();
        Task<UtilizadorDto?> CriarUtilizadorAsync(CriarUtilizadorAdminRequestDto dto);
    }
}
