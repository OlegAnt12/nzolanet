using NzolaWebAPI.DTOs.Denuncia;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Mappers
{
    public static class DenunciaMappers
    {
        public static DenunciaDto ToDenunciaDto(this Denuncia model)
        {
            return new DenunciaDto
            {
                Id = model.Id,
                TipoEntidade = model.TipoEntidade,
                IdEntidade = model.IdEntidade,
                Motivo = model.Motivo,
                Descricao = model.Descricao,
                DenuncianteId = model.DenuncianteId,
                NomeDenunciante = model.Denunciante?.NomeCompleto,
                DataDenuncia = model.DataDenuncia,
                EstadoDenuncia = model.EstadoDenuncia
            };
        }

        public static Denuncia ToDenunciaFromCriarDto(this CriarDenunciaDto dto)
        {
            return new Denuncia
            {
                TipoEntidade = dto.TipoEntidade,
                IdEntidade = dto.IdEntidade,
                Motivo = dto.Motivo,
                Descricao = dto.Descricao,
                DenuncianteId = dto.DenuncianteId,
                DataDenuncia = DateTime.Now,
                EstadoDenuncia = Models.Enums.EstadoDenuncia.Pendente
            };
        }
    }
}
