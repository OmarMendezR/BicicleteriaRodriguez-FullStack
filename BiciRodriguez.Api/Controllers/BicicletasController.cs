using BiciRodriguez.Api.Models; //Referencia de los modelos a usar
using BiciRodriguez.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Controllers
{
    // Atributos de clase
    [Route("api/[controller]")]
    [ApiController]
    public class BicicletasController : ControllerBase
    {
        private readonly BiciContext _context; // Puente con la DB

        // Constructor (Inyeccion de dependencias)
        public BicicletasController(BiciContext context)
        {
            _context = context;
        }
        #region METODOS DE LECTURA (GET)

        // Acción para listar todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BicicletaResponseDto>>> GetBicicletas()
        {
            // Usamos Select para mapear a DTO: limpia el Swagger y es más eficiente en SQL
            return await _context.Bicicletas
                .Select(b => new BicicletaResponseDto
                {
                    BicicletaId = b.BicicletaId,
                    Marca = b.Marca,
                    NumeroMarco = b.NumeroMarco,
                    ClienteId = b.ClienteId,
                    Color = b.Color,
                    TipoBicicleta = b.TipoBicicleta
                })
                .ToListAsync();
        }

        // GET by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BicicletaResponseDto>> GetBicicleta(int id)
        {
            // Buscamos la entidad
            var b = await _context.Bicicletas.FindAsync(id);

            if (b == null)
            {
                return NotFound(new { mensaje = $"La bicicleta con ID {id} no fue encontrada." });
            }

            // Mapeamos manualmente a DTO para la respuesta
            var response = new BicicletaResponseDto
            {
                BicicletaId = b.BicicletaId,
                Marca = b.Marca,
                NumeroMarco = b.NumeroMarco,
                ClienteId = b.ClienteId,
                Color = b.Color,
                TipoBicicleta = b.TipoBicicleta
            };

            return Ok(response);
        }

        // GET por Cliente (Ya lo tenías bien, lo mantenemos por consistencia)
        [HttpGet("Cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<BicicletaResponseDto>>> GetBicicletasPorCliente(int clienteId)
        {
            var bicicletas = await _context.Bicicletas
                .Where(b => b.ClienteId == clienteId)
                .Select(b => new BicicletaResponseDto
                {
                    BicicletaId = b.BicicletaId,
                    Marca = b.Marca,
                    NumeroMarco = b.NumeroMarco,
                    ClienteId = b.ClienteId,
                    Color = b.Color,
                    TipoBicicleta = b.TipoBicicleta
                })
                .ToListAsync();

            if (bicicletas == null || !bicicletas.Any())
            {
                return NotFound($"El cliente {clienteId} no tiene bicicletas registradas.");
            }

            return Ok(bicicletas);
        }
        #endregion
        #region MËTODOS DE ESCRITURA (POST)
        // POST: api/Bicicleteria
        [HttpPost]
        public async Task<ActionResult<BicicletaResponseDto>> PostBicicleta(BicicletaResponseDto biciDto)
        {
            // 1. EXTRAER IDENTIDAD (Preparado para Token)
            var userIdClaim = User.FindFirst("id")?.Value ?? "1";
            int currentUserId = int.Parse(userIdClaim);

            // 2. VALIDACIÓN: Evitar duplicados de marco
            bool existeMarco = await _context.Bicicletas
                .AnyAsync(b => b.NumeroMarco == biciDto.NumeroMarco);

            if (existeMarco)
            {
                return BadRequest($"Error: El número de marco '{biciDto.NumeroMarco}' ya está registrado.");
            }

            // 3. MAPEO: De DTO a Entidad
            var nuevaBici = new Bicicleta
            {
                Marca = biciDto.Marca,
                NumeroMarco = biciDto.NumeroMarco,
                ClienteId = (biciDto.ClienteId == 0) ? null : biciDto.ClienteId,
                Color = string.IsNullOrWhiteSpace(biciDto.Color) ? "N/A" : biciDto.Color,
                TipoBicicleta = string.IsNullOrWhiteSpace(biciDto.TipoBicicleta) ? "General" : biciDto.TipoBicicleta,

                // Valores automáticos de sistema
                Modelo = "N/A",
                FechaRegistro = DateTime.UtcNow, // Usamos UtcNow por estándar
                UltimaModificacion = DateTime.UtcNow,

                // ASIGNACIÓN AUTOMÁTICA DEL USUARIO
                CreadoPorUsuarioId = currentUserId
            };

            try
            {
                _context.Bicicletas.Add(nuevaBici);
                await _context.SaveChangesAsync();

                biciDto.BicicletaId = nuevaBici.BicicletaId;
                return CreatedAtAction(nameof(GetBicicleta), new { id = nuevaBici.BicicletaId }, biciDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // PUT: api/Bicicleteria/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBicicleta(int id, BicicletaResponseDto biciDto)
        {
            if (id != biciDto.BicicletaId)
            {
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo.");
            }

            var bicicletaExistente = await _context.Bicicletas.FindAsync(id);
            if (bicicletaExistente == null)
            {
                return NotFound($"La bicicleta con ID {id} no existe.");
            }

            // BLINDAJE: Validar ClienteID si se proporciona
            if (biciDto.ClienteId.HasValue && biciDto.ClienteId != 0)
            {
                var clienteExiste = await _context.Clientes.AnyAsync(c => c.ClienteId == biciDto.ClienteId);
                if (!clienteExiste) return BadRequest($"El Cliente ID {biciDto.ClienteId} no existe.");
            }

            // BLINDAJE: Validar duplicado de marco (solo si cambió el marco original)
            if (bicicletaExistente.NumeroMarco != biciDto.NumeroMarco)
            {
                bool marcoEnUso = await _context.Bicicletas
                    .AnyAsync(b => b.NumeroMarco == biciDto.NumeroMarco && b.BicicletaId != id);
                if (marcoEnUso) return BadRequest("El nuevo número de marco ya pertenece a otra bicicleta.");
            }

            // ACTUALIZACIÓN DE CAMPOS
            bicicletaExistente.Marca = biciDto.Marca;
            bicicletaExistente.NumeroMarco = biciDto.NumeroMarco;
            bicicletaExistente.ClienteId = (biciDto.ClienteId == 0) ? null : biciDto.ClienteId;
            bicicletaExistente.Color = string.IsNullOrWhiteSpace(biciDto.Color) ? "N/A" : biciDto.Color;
            bicicletaExistente.TipoBicicleta = string.IsNullOrWhiteSpace(biciDto.TipoBicicleta) ? "General" : biciDto.TipoBicicleta;
            bicicletaExistente.UltimaModificacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        #endregion
        #region METODOS DE ELIMINACION (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBicicleta(int id)
        {
            // 1. Buscamos la entidad original
            var bicicleta = await _context.Bicicletas.FindAsync(id);

            if (bicicleta == null)
            {
                return NotFound(new { mensaje = $"No se pudo eliminar: La bicicleta con ID {id} no existe." });
            }

            // 2. VALIDACIÓN DE NEGOCIO (Opcional pero recomendado)
            // Aquí podrías verificar si tiene registros relacionados antes de proceder.

            try
            {
                _context.Bicicletas.Remove(bicicleta);
                await _context.SaveChangesAsync();

                // Devolvemos un 200 OK con un objeto anónimo limpio
                return Ok(new
                {
                    mensaje = "Eliminación exitosa",
                    idEliminado = id,
                    marcoEliminado = bicicleta.NumeroMarco
                });
            }
            catch (Exception ex)
            {
                // Capturamos errores de integridad referencial (ej: si ya tiene mantenimientos)
                return StatusCode(500, new
                {
                    mensaje = "Error al eliminar el registro. Es posible que tenga información relacionada en otras tablas.",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        #endregion
    }
}