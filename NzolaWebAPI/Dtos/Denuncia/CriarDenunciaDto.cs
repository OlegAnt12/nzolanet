using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.Denuncia
{
    public class CriarDenunciaDto
    {
        public TipoEntidade TipoEntidade { get; set; }
        public int IdEntidade { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int DenuncianteId { get; set; }
    }
}
