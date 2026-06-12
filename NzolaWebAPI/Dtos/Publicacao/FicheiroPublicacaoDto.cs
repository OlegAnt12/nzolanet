using System;

namespace NzolaWebAPI.DTOs.Publicacao
{
    public class FicheiroPublicacaoDto
    {
        public int Id { get; set; }
        public string CaminhoFicheiro { get; set; } = string.Empty;
        public string TipoMime { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public DateTime DataUpload { get; set; }
    }
}
