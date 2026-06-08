using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.Repositories
{
    public class ConteudoPublicacaoRepository : IConteudoPublicacaoRepository
    {
        private readonly ContextoBDNzola _contexto;

        public ConteudoPublicacaoRepository(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        public async Task AdicionarAsync(List<ConteudoPublicacao> conteudos)
        {
            await _contexto.ConteudosPublicacao.AddRangeAsync(conteudos);
        }
    }
}
