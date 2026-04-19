using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Authorization; // Agregado
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protegido globalmente
    public class ProductosController : ControllerBase
    {
        private readonly IProductosService _service;

        public ProductosController(IProductosService service) => _service = service;

        #region METODOS DE LECTURA

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
            => Ok(await _service.GetAllAsync());

        [HttpPost]
        [Authorize(Roles = "Administrador,Mecanico")] // Solo personal autorizado
        public async Task<ActionResult<ProductoDto>> PostProducto(ProductoDto dto)
        {
            try
            {
                // Extraemos el ID de forma segura del token
                var userIdClaim = User.FindFirst("id")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { mensaje = "Token inválido" });

                var resultado = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetProductos), new { id = resultado.ProductoId }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        #endregion

        #region METODOS DE ACTUALIZACION

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<IActionResult> PutProducto(int id, ProductoDto dto)
        {
            if (id != dto.ProductoId) return BadRequest(new { mensaje = "ID no coincide." });

            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { mensaje = "Token inválido" });

                await _service.UpdateAsync(dto, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        #endregion

        #region METODOS DE ELIMINACION

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] // Solo el administrador puede eliminar del inventario
        public async Task<IActionResult> DeleteProducto(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        #endregion
    }
}