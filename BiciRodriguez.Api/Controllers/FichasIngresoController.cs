using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FichasIngresoController : ControllerBase
    {
        private readonly IFichasService _service;

        public FichasIngresoController(IFichasService service)
        {
            _service = service;
        }

        #region METODOS DE CREACION POST
        [HttpPost]
        public async Task<ActionResult<FichasIngreso>> PostFicha(FichaIngresoCreateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")?.Value ?? "1");
                var resultado = await _service.CreateFichaAsync(dto, userId);
                return Ok(resultado);
            }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }

        [HttpPost("repuesto")]
        public async Task<IActionResult> AgregarRepuesto(DetalleRepuestoCreateDto dto)
        {
            try { return Ok(await _service.AddRepuestoAsync(dto)); }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }

        [HttpPost("mano-obra")]
        public async Task<IActionResult> AgregarManoObra(DetalleManoObraCreateDto dto)
        {
            try { return Ok(await _service.AddManoObraAsync(dto)); }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }
        #endregion

        #region METODOS DE LECTURA GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FichaIngresoResponseDto>>> GetFichas()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}/resumen")]
        public async Task<IActionResult> GetResumenFicha(int id)
        {
            var resumen = await _service.GetResumenAsync(id);
            if (resumen == null) return NotFound("Ficha no encontrada.");
            return Ok(resumen);
        }
        #endregion

        #region METODOS DE MODIFICACION
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            try
            {
                var exito = await _service.UpdateEstadoAsync(id, nuevoEstado);
                if (!exito) return NotFound("Ficha no encontrada.");
                return Ok(new { mensaje = $"Estado actualizado a {nuevoEstado}" });
            }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }
        #endregion
    }
}