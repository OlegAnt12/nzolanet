using NzolaWebAPI.DTOs.Denuncia;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Services
{
    public class DenunciaService : IDenunciaService
    {
        private readonly IDenunciaRepository _denunciaRepo;

        public DenunciaService(IDenunciaRepository denunciaRepo)
        {
            _denunciaRepo = denunciaRepo;
        }

        public async Task<DenunciaDto?> CriarDenunciaAsync(CriarDenunciaDto dto)
        {
            var denuncia = dto.ToDenunciaFromCriarDto();
            await _denunciaRepo.AdicionarAsync(denuncia);
            await _denunciaRepo.SalvarAsync();
            return denuncia.ToDenunciaDto();
        }

        public async Task<List<DenunciaDto>> ListarTodasAsync()
        {
            var denuncias = await _denunciaRepo.ListarTodasAsync();
            return denuncias.Select(d => d.ToDenunciaDto()).ToList();
        }

        public async Task<List<DenunciaDto>> ListarPorEntidadeAsync(TipoEntidade tipoEntidade, int idEntidade)
        {
            var denuncias = await _denunciaRepo.ListarPorEntidadeAsync(tipoEntidade, idEntidade);
            return denuncias.Select(d => d.ToDenunciaDto()).ToList();
        }

        public async Task<DenunciaDto?> AtualizarEstadoDenunciaAsync(int id, EstadoDenuncia novoEstado)
        {
            var denuncia = await _denunciaRepo.ObterPorIdAsync(id);
            if (denuncia == null) return null;

            denuncia.EstadoDenuncia = novoEstado;
            _denunciaRepo.Atualizar(denuncia);
            await _denunciaRepo.SalvarAsync();

            return denuncia.ToDenunciaDto();
        }
    }
}
