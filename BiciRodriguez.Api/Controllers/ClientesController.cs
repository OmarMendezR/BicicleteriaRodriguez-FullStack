using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protegido globalmente
    public class ClientesController : ControllerBase
    {
        private readonly IClientesService _service;

        public ClientesController(IClientesService service) => _service = service;

        #region READ
        [HttpGet]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            var cliente = await _service.GetByIdAsync(id);
            return cliente == null ? NotFound(new { mensaje = "Cliente no encontrado" }) : Ok(cliente);
        }
        #endregion

        #region WRITE
        [HttpPost]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<ActionResult<ClienteDto>> PostCliente(ClienteDto dto)
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized(new { mensaje = "Token inválido" });

            var resultado = await _service.CreateAsync(dto, currentUserId);
            return CreatedAtAction(nameof(GetCliente), new { id = resultado.ClienteId }, resultado);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<IActionResult> PutCliente(int id, ClienteDto dto)
        {
            if (id != dto.ClienteId) return BadRequest(new { mensaje = "ID mismatch" });

            var actualizado = await _service.UpdateAsync(id, dto);
            return actualizado ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] // Solo Admin elimina clientes
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                return await _service.DeleteAsync(id) ? Ok(new { mensaje = "Eliminado" }) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        #endregion
    }
}