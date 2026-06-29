using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Interfaces;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosSeguirController : ControllerBase
    {
        private readonly IPedidoSeguirService _pedidoSeguirService;

        public PedidosSeguirController(IPedidoSeguirService pedidoSeguirService)
        {
            _pedidoSeguirService = pedidoSeguirService;
        }

        [HttpPost("{seguidorId}/{seguidoId}")]
        public async Task<IActionResult> SolicitarSeguimento(int seguidorId, int seguidoId)
        {
            var resultado = await _pedidoSeguirService.SolicitarSeguimentoAsync(seguidorId, seguidoId);
            if (resultado == null)
                return BadRequest("Não foi possível criar o pedido. Verifique se já segue este perfil ou se já existe um pedido pendente.");
            return Ok(resultado);
        }

        [HttpGet("pendentes/{utilizadorId}")]
        public async Task<IActionResult> ListarPendentes(int utilizadorId)
        {
            var pedidos = await _pedidoSeguirService.ListarPendentesAsync(utilizadorId);
            return Ok(pedidos);
        }

        [HttpPut("{pedidoId}/aceitar")]
        public async Task<IActionResult> AceitarPedido(int pedidoId)
        {
            var resultado = await _pedidoSeguirService.AceitarPedidoAsync(pedidoId);
            if (resultado == null) return NotFound("Pedido não encontrado ou já processado.");
            return Ok(resultado);
        }

        [HttpPut("{pedidoId}/rejeitar")]
        public async Task<IActionResult> RejeitarPedido(int pedidoId)
        {
            var resultado = await _pedidoSeguirService.RejeitarPedidoAsync(pedidoId);
            if (resultado == null) return NotFound("Pedido não encontrado ou já processado.");
            return Ok(resultado);
        }
    }
}
