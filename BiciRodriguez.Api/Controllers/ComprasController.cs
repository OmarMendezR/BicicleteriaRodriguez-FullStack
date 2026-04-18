using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ComprasController : ControllerBase
    {
        private readonly IComprasService _service;

        public ComprasController(IComprasService service)
        {
            _service = service;
        }

        #region PLANIFICACIÓN DE COMPRAS

        [HttpGet("alertas-stock")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<IEnumerable<SugerenciaPedidoDto>>> GetAlertasStock()
        {
            try
            {
                var alertas = await _service.GetProductosEnAlertaAsync();
                return Ok(alertas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener alertas: " + ex.Message });
            }
        }

        [HttpPost("crear-borrador")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CrearPedidoBorrador(List<SugerenciaPedidoDto> items)
        {
            // Verificamos identidad del token
            var userIdClaim = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdClaim, out _))
                return Unauthorized(new { mensaje = "Token inválido" });

            if (items == null || !items.Any())
                return BadRequest(new { mensaje = "La lista de productos no puede estar vacía." });

            try
            {
                var pedidoId = await _service.CrearPedidoBorradorAsync(items);

                if (pedidoId == 0)
                    return BadRequest(new { mensaje = "No se pudo generar el borrador del pedido." });

                return CreatedAtAction(nameof(RecibirPedido), new { id = pedidoId }, new { id = pedidoId, mensaje = "Borrador de pedido creado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al crear el borrador: " + ex.Message });
            }
        }

        #endregion

        #region OPERACIONES DE ALMACÉN

        [HttpPut("recibir/{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RecibirPedido(int id)
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdClaim, out _))
                return Unauthorized(new { mensaje = "Token inválido" });

            try
            {
                var resultado = await _service.RecibirPedidoAsync(id);

                if (!resultado)
                {
                    return NotFound(new { mensaje = "El pedido no existe o ya ha sido marcado como 'Recibido'." });
                }

                return Ok(new { mensaje = "Inventario actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al procesar la compra: " + ex.Message });
            }
        }

        #endregion
    }
}