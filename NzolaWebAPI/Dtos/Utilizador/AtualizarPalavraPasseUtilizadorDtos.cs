using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.Dtos.Utilizador
{
    public class AtualizarPalavraPasseUtilizadorDtos
    {
        public int Id { get; set; }

        public string PalavraPasseAtual { get; set; }

        public string NovaPalavraPasse { get; set; }
    }
}