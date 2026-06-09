using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Utilizador
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string PalavraPasse { get; set; } = string.Empty;
        
    }
}