using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductosService _service;

        public ProductosController(IProductosService service)
        {
            _service = service;
        }

        #region METODOS GET DE LECTURA
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            return Ok(await _service.GetAllAsync());
        }
        #endregion

        #region METODOS POST DE CREACION
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> PostProducto(ProductoDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value ?? "1";
                int currentUserId = int.Parse(userIdClaim);

                var resultado = await _service.CreateAsync(dto, currentUserId);

                // Nota: He usado nameof(GetProductos) para el CreatedAtAction
                return CreatedAtAction(nameof(GetProductos), new { id = resultado.ProductoId }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        #endregion
    }
}