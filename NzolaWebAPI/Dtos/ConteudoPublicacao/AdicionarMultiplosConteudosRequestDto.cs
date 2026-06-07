using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.ConteudoPublicacao
{
    public class AdicionarMultiplosConteudosRequestDto
    {
        public List<ItemConteudoRequestDto> Elementos { get; set; } = new();
    }
}
