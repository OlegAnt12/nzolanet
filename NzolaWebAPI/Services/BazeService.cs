using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Baze;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Services
{
    public class BazeService : IBazeService
    {
        private readonly IBazeRepository _bazeRepo;
        private readonly IPublicacaoRepository _publicacaoRepo; // Certifica-te de que tens este repo injetado

        //private readonly IUtilizadorRepository _utilizadorRepo; // Ou injeta o DbContext se preferires validar direto
        private readonly ContextoBDNzola _contexto;

        public BazeService(
            IBazeRepository bazeRepo,
            IPublicacaoRepository publicacaoRepo,
            /*IUtilizadorRepository utilizadorRepo*/
            ContextoBDNzola contexto
        )
        {
            _bazeRepo = bazeRepo;
            _publicacaoRepo = publicacaoRepo;
            //_utilizadorRepo = utilizadorRepo;
            _contexto = contexto;
        }

        public async Task<BazeResultadoDto> AlternarBazeAsync(int publicacaoId, int utilizadorId)
        {
            var resultado = new BazeResultadoDto();

            // 1. Validações
            //bool utilizadorExiste = await _utilizadorRepo.ExisteAsync(utilizadorId); // Exemplo de método no teu repo de utilizadores
            bool utilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Id == utilizadorId
            );
            if (!utilizadorExiste)
            {
                resultado.ErroMensagem = "Este Utilizador Não Existe";
                return resultado;
            }

            var publicacao = await _publicacaoRepo.SelecionarAsync(publicacaoId);
            if (publicacao == null)
            {
                resultado.ErroMensagem = "Esta Publicação Não Existe";
                return resultado;
            }

            // 2. Lógica do Toggle (Dar ou Remover Baze)
            var bazeExistente = await _bazeRepo.ObterPorPublicacaoEUtilizadorAsync(
                publicacaoId,
                utilizadorId
            );

            if (bazeExistente != null)
            {
                _bazeRepo.Remover(bazeExistente);

                if (publicacao.QuantidadeBazes > 0)
                    publicacao.QuantidadeBazes--;

                await _bazeRepo.SalvarAsync();
                await _publicacaoRepo.SalvarAsync(); // Atualiza os contadores da publicação

                resultado.FoiRemovido = true;
                resultado.QuantidadeBazes = publicacao.QuantidadeBazes;
                return resultado;
            }

            // Criar novo Baze
            Baze baze = new();
            baze.PublicacaoId = publicacaoId;
            baze.UtilizadorId = utilizadorId;
            baze.EstadoBaze = (EstadoBaze)1;
            baze.DataInteracao = DateTime.Now;
            publicacao.QuantidadeBazes++;

            await _bazeRepo.AdicionarAsync(baze);
            await _bazeRepo.SalvarAsync();
            await _publicacaoRepo.SalvarAsync();

            resultado.FoiRemovido = false;
            resultado.QuantidadeBazes = publicacao.QuantidadeBazes;
            resultado.BazeDto = baze.ToBazeDto();

            return resultado;
        }
    }
}
