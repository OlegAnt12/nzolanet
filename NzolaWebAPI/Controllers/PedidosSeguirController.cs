using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.DTOs.Seguidor;
using NzolaWebAPI.Interfaces;

namespace NzolaWebAPI.Controllers
{
    /// <summary>
    /// Controlador para gestão de pedidos de seguimento entre utilizadores (utilizado quando o perfil é privado).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosSeguirController : ControllerBase
    {
        private readonly IPedidoSeguirService _pedidoSeguirService;

        public PedidosSeguirController(IPedidoSeguirService pedidoSeguirService)
        {
            _pedidoSeguirService = pedidoSeguirService;
        }

        /// <summary>
        /// Envia um pedido para seguir um utilizador com perfil privado.
        /// </summary>
        /// <param name="seguidorId">ID do utilizador que solicita seguir</param>
        /// <param name="seguidoId">ID do utilizador a ser seguido</param>
        /// <returns>Pedido de seguimento criado</returns>
        [HttpPost("{seguidorId}/{seguidoId}")]
        [ProducesResponseType(typeof(PedidoSeguirDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SolicitarSeguimento(int seguidorId, int seguidoId)
        {
            var resultado = await _pedidoSeguirService.SolicitarSeguimentoAsync(seguidorId, seguidoId);
            if (resultado == null)
                return BadRequest("Não foi possível criar o pedido. Verifique se já segue este perfil ou se já existe um pedido pendente.");
            return Ok(resultado);
        }

        /// <summary>
        /// Lista os pedidos de seguimento pendentes de um utilizador.
        /// </summary>
        /// <param name="utilizadorId">ID do utilizador</param>
        /// <returns>Lista de pedidos pendentes</returns>
        [HttpGet("pendentes/{utilizadorId}")]
        [ProducesResponseType(typeof(IEnumerable<PedidoSeguirDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarPendentes(int utilizadorId)
        {
            var pedidos = await _pedidoSeguirService.ListarPendentesAsync(utilizadorId);
            return Ok(pedidos);
        }

        /// <summary>
        /// Aceita um pedido de seguimento pendente.
        /// </summary>
        /// <param name="pedidoId">ID do pedido de seguimento</param>
        /// <returns>Pedido de seguimento aceite</returns>
        [HttpPut("{pedidoId}/aceitar")]
        [ProducesResponseType(typeof(PedidoSeguirDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AceitarPedido(int pedidoId)
        {
            var resultado = await _pedidoSeguirService.AceitarPedidoAsync(pedidoId);
            if (resultado == null) return NotFound("Pedido não encontrado ou já processado.");
            return Ok(resultado);
        }

        /// <summary>
        /// Rejeita um pedido de seguimento pendente.
        /// </summary>
        /// <param name="pedidoId">ID do pedido de seguimento</param>
        /// <returns>Pedido de seguimento rejeitado</returns>
        [HttpPut("{pedidoId}/rejeitar")]
        [ProducesResponseType(typeof(PedidoSeguirDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RejeitarPedido(int pedidoId)
        {
            var resultado = await _pedidoSeguirService.RejeitarPedidoAsync(pedidoId);
            if (resultado == null) return NotFound("Pedido não encontrado ou já processado.");
            return Ok(resultado);
        }
    }
}
