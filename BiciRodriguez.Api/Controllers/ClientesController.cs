using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClientesService _service;

        public ClientesController(IClientesService service)
        {
            _service = service;
        }

        #region METODOS DE LECTURA (GET)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            var clientes = await _service.GetAllAsync();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            var cliente = await _service.GetByIdAsync(id);
            if (cliente == null) return NotFound(new { mensaje = $"El cliente {id} no existe." });
            return Ok(cliente);
        }
        #endregion

        #region METODOS DE ESCRITURA (POST, PUT)
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> PostCliente(ClienteDto dto)
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? "1";
            int currentUserId = int.Parse(userIdClaim);

            var resultado = await _service.CreateAsync(dto, currentUserId);
            return CreatedAtAction(nameof(GetCliente), new { id = resultado.ClienteId }, resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, ClienteDto dto)
        {
            if (id != dto.ClienteId) return BadRequest("ID mismatch.");

            var actualizado = await _service.UpdateAsync(id, dto);
            if (!actualizado) return NotFound();

            return NoContent();
        }
        #endregion

        #region METODOS DE ELIMINACION (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                var eliminado = await _service.DeleteAsync(id);
                if (!eliminado) return NotFound();

                return Ok(new { mensaje = "Client deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        #endregion
    }
}