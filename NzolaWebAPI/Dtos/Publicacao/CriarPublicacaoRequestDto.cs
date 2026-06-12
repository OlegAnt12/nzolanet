using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NzolaWebAPI.DTOs.Publicacao
{
    public class CriarPublicacaoRequestDto
    {
        public string Texto { get; set; } = string.Empty;

        public List<IFormFile>? Ficheiros { get; set; }
    }
}
