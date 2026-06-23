using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Seguidor;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class UtilizadorSimplificadoDto
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string NomeUtilizador { get; set; } = string.Empty;
        public string? FotoPerfil { get; set; }
        public int AutorQuantidadePublicacoes { get; set; }
        public List<SeguidorFeedDto> Seguidores { get; set; } = new List<SeguidorFeedDto>();
        public List<SeguidorFeedDto> Seguidos { get; set; } = new List<SeguidorFeedDto>();
    }
}
