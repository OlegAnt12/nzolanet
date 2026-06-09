using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.DTOs.Comentario;

namespace NzolaWebAPI.DTOs.Publicacao
{
    public class CriarPublicacaoRequestDto
    {
        public int AutorId { get; set; }
        public List<ItemConteudoRequestDto> Conteudos { get; set; } = new List<ItemConteudoRequestDto>();
    }
}