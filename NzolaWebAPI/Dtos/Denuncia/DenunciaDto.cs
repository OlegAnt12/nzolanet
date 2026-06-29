using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.Denuncia
{
    public class DenunciaDto
    {
        public int Id { get; set; }
        public TipoEntidade TipoEntidade { get; set; }
        public int IdEntidade { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int DenuncianteId { get; set; }
        public string? NomeDenunciante { get; set; }
        public DateTime DataDenuncia { get; set; }
        public EstadoDenuncia EstadoDenuncia { get; set; }
    }
}
