using BiciRodriguez.Api.Interfaces;
using BiciRodriguez.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Solo usuarios logueados pueden ver finanzas
    public class BalancesController : ControllerBase
    {
        private readonly IBalancesService _balancesService;

        public BalancesController(IBalancesService balancesService)
        {
            _balancesService = balancesService;
        }

        [HttpGet("historial")]
        public async Task<IActionResult> GetHistorial()
        {
            var historial = await _balancesService.GetHistorialAsync();
            return Ok(historial);
        }

        [HttpPost("cierre-manual")]
        public async Task<IActionResult> EjecutarCierreManual()
        {
            // Buscamos el claim exacto que definiste en AuthService ("id")
            var usuarioIdClaim = User.FindFirst("id")?.Value;

            int? usuarioId = null;
            if (!string.IsNullOrEmpty(usuarioIdClaim) && int.TryParse(usuarioIdClaim, out int id))
            {
                usuarioId = id;
            }

            // El resto del código se mantiene igual
            var resultado = await _balancesService.GenerarCierreDiarioAsync(usuarioId);

            if (resultado)
                return Ok(new { mensaje = "Cierre diario generado exitosamente.", registradoPor = usuarioId });

            return BadRequest(new { mensaje = "No se pudo generar el cierre." });
        }
    }
}