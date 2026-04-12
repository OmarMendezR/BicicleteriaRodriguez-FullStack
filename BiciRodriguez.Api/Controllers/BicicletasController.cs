using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BicicletasController : ControllerBase
    {
        private readonly IBicicletasService _service;

        // Constructor con Inyección de Dependencias de la Interfaz del Servicio
        public BicicletasController(IBicicletasService service)
        {
            _service = service;
        }

        #region METODOS DE LECTURA (GET)

        // Acción para listar todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BicicletaResponseDto>>> GetBicicletas()
        {
            var bicicletas = await _service.GetAllAsync();
            return Ok(bicicletas);
        }

        // GET by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BicicletaResponseDto>> GetBicicleta(int id)
        {
            var b = await _service.GetByIdAsync(id);

            if (b == null)
            {
                return NotFound(new { mensaje = $"La bicicleta con ID {id} no fue encontrada." });
            }

            return Ok(b);
        }

        // GET por Cliente
        [HttpGet("Cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<BicicletaResponseDto>>> GetBicicletasPorCliente(int clienteId)
        {
            var bicicletas = await _service.GetByClienteIdAsync(clienteId);

            if (bicicletas == null || !bicicletas.Any())
            {
                return NotFound(new { mensaje = $"El cliente {clienteId} no tiene bicicletas registradas." });
            }

            return Ok(bicicletas);
        }

        #endregion

        #region MËTODOS DE ESCRITURA (POST)

        // POST: api/Bicicletas
        [HttpPost]
        public async Task<ActionResult<BicicletaResponseDto>> PostBicicleta(BicicletaResponseDto biciDto)
        {
            // Extraer identidad (Preparamos para Token, por ahora default 1)
            var userIdClaim = User.FindFirst("id")?.Value ?? "1";
            int currentUserId = int.Parse(userIdClaim);

            try
            {
                var resultado = await _service.CreateAsync(biciDto, currentUserId);
                return CreatedAtAction(nameof(GetBicicleta), new { id = resultado.BicicletaId }, resultado);
            }
            catch (Exception ex)
            {
                // El servicio lanza errores lógicos (ej: marco duplicado), aquí los devolvemos como BadRequest
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: api/Bicicletas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBicicleta(int id, BicicletaResponseDto biciDto)
        {
            if (id != biciDto.BicicletaId)
            {
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del cuerpo." });
            }

            try
            {
                var actualizado = await _service.UpdateAsync(id, biciDto);
                if (!actualizado) return NotFound(new { mensaje = $"La bicicleta con ID {id} no existe." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        #endregion

        #region METODOS DE ELIMINACION (DELETE)

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBicicleta(int id)
        {
            try
            {
                var eliminado = await _service.DeleteAsync(id);
                if (!eliminado) return NotFound(new { mensaje = $"No se pudo eliminar: ID {id} no encontrado." });

                return Ok(new { mensaje = "Eliminación exitosa", idEliminado = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al eliminar el registro.",
                    detalle = ex.Message
                });
            }
        }

        #endregion
    }
}