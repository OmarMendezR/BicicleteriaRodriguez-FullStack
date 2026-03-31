using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly BiciContext _context;

        public ClientesController(BiciContext context)
        {
            _context = context;
        }

        #region METODOS DE LECTURA (GET)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes() // Cambiado a plural para diferenciar
        {
            return await _context.Clientes
                .Select(c => new ClienteDto
                {
                    ClienteId = c.ClienteId,
                    
                    Nombre = c.NombreCompleto, 
                    Telefono = c.Telefono ?? string.Empty,
                    Correo = c.Email ?? string.Empty, 
                    Direccion = c.Direccion ?? string.Empty
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            var c = await _context.Clientes.FindAsync(id);

            if (c == null)
            {
                return NotFound(new { mensaje = $"El cliente {id} no existe." });
            }

            return new ClienteDto
            {
                ClienteId = c.ClienteId,
                Nombre = c.NombreCompleto,
                Telefono = c.Telefono ?? string.Empty,
                Correo = c.Email ?? string.Empty,
                Direccion = c.Direccion ?? string.Empty
            };
        } // <-- FALTA ESTA LLAVE (Cierra GetCliente por ID)
        #endregion // <-- FALTA ESTA LLAVE (Cierra la región de lectura)
        #region METODOS DE ESCRITURA (POST, PUT)
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> PostCliente(ClienteDto dto)
        {
            // Lógica para el Token (lista para cuando implementemos JWT)
            // Buscamos el Claim de nombre "sub" o "id" en el token
            var userIdClaim = User.FindFirst("id")?.Value ?? "1";
            int currentUserId = int.Parse(userIdClaim);

            var nuevoCliente = new Cliente
            {
                NombreCompleto = $"{dto.Nombre} {dto.Apellido}".Trim(),
                Telefono = dto.Telefono,
                Email = dto.Correo,
                Direccion = dto.Direccion,
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                AutorizaDatos = true,

                // Usamos el ID extraído (del token o el default)
                CreadoPorUsuarioId = currentUserId
            };

            _context.Clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            dto.ClienteId = nuevoCliente.ClienteId;
            return CreatedAtAction(nameof(GetCliente), new { id = dto.ClienteId }, dto);
        }
        // Metodo de actualizacion por ID (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, ClienteDto dto)
        {
            if (id != dto.ClienteId) return BadRequest("ID mismatch.");

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            // Mapeo inverso: Del DTO a la Entidad
            cliente.NombreCompleto = $"{dto.Nombre} {dto.Apellido}".Trim();
            cliente.Telefono = dto.Telefono;
            cliente.Email = dto.Correo;
            cliente.Direccion = dto.Direccion;

            // Auditoría de modificación (preparado para el token)
            cliente.UltimaModificacion = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // 204: Todo bien, nada que devolver
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating client: {ex.Message}");
            }
        }
        #endregion
        #region METODOS DE ELIMINACION (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            // REGLA DE NEGOCIO: No borrar clientes con bicicletas vinculadas
            var tieneBicis = await _context.Bicicletas.AnyAsync(b => b.ClienteId == id);
            if (tieneBicis)
            {
                return BadRequest("Cannot delete: This client has registered bicycles. Delete or reassign them first.");
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Client deleted successfully." });
        }
        #endregion
    }
}