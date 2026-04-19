using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Authorization; // Agregado
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Agregado: Solo usuarios del sistema ven proveedores
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedoresService _service;

        public ProveedoresController(IProveedoresService service)
        {
            _service = service;
        }

        #region METODOS GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorDto>>> GetProveedores()
        {
            try
            {
                var proveedores = await _service.GetAllAsync();
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener proveedores", detalle = ex.Message });
            }
        }
        #endregion
    }
}