using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FichasIngresoController : ControllerBase
    {
        private readonly IFichasService _service;
        public FichasIngresoController(IFichasService service) => _service = service;

        #region METODOS DE LECTURA (GET)
        [HttpGet]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<ActionResult<IEnumerable<FichaIngresoResponseDto>>> GetFichas()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}/resumen")]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<IActionResult> GetResumen(int id)
        {
            var resumen = await _service.GetResumenAsync(id);
            return resumen == null ? NotFound() : Ok(resumen);
        }
        #endregion

        #region METODOS DE ESCRITURA (POST)
        [HttpPost]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<ActionResult<FichaIngresoResponseDto>> PostFicha(FichaIngresoCreateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")?.Value ?? "1");
                var resultado = await _service.CreateFichaAsync(dto, userId);
                return CreatedAtAction(nameof(GetFichas), new { id = resultado.FichaId }, resultado);
            }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }

        [HttpPost("repuesto")]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<IActionResult> AgregarRepuesto(DetalleRepuestoCreateDto dto)
        {
            try { return Ok(await _service.AddRepuestoAsync(dto)); }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }

        [HttpPost("mano-obra")]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<IActionResult> AgregarManoObra(DetalleManoObraCreateDto dto)
        {
            try { return Ok(await _service.AddManoObraAsync(dto)); }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }
        #endregion

        #region METODOS DE MODIFICACION (PATCH/PUT)
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            try
            {
                var exito = await _service.UpdateEstadoAsync(id, nuevoEstado);
                return exito ? Ok(new { mensaje = "Estado actualizado" }) : NotFound();
            }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }
        #endregion
    }
}