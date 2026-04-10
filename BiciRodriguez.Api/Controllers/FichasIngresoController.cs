using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FichasIngresoController : ControllerBase
    {
        private readonly BiciContext _context;

        public FichasIngresoController(BiciContext context)
        {
            _context = context;
        }
        #region METODOS DE CREACION POST
        // POST: api/FichasIngreso
        [HttpPost]
        public async Task<ActionResult<FichasIngreso>> PostFicha(FichaIngresoCreateDto dto)
        {
            // 1. VALIDACIÓN DE SEGURIDAD: ¿La bici es del cliente?
            var bicicleta = await _context.Bicicletas
                .FirstOrDefaultAsync(b => b.BicicletaId == dto.BicicletaId && b.ClienteId == dto.ClienteId);

            if (bicicleta == null)
            {
                return BadRequest("La bicicleta seleccionada no pertenece al cliente especificado.");
            }

            // 2. IDENTIDAD (Usuario que registra la entrada)
            var userIdClaim = User.FindFirst("id")?.Value ?? "1";
            int currentUserId = int.Parse(userIdClaim);

            // 3. MAPEO
            var nuevaFicha = new FichasIngreso
            {
                ClienteId = dto.ClienteId,
                BicicletaId = dto.BicicletaId,
                MecanicoId = dto.MecanicoId,
                Observaciones = dto.Observaciones,
                Estado = dto.Estado,
                FechaEntrada = DateTime.UtcNow,
                UltimaModificacion = DateTime.UtcNow,
                CreadoPorUsuarioId = currentUserId
            };

            try
            {
                _context.FichasIngresos.Add(nuevaFicha);
                await _context.SaveChangesAsync();

                // Por ahora devolvemos el objeto, luego crearemos un DTO de respuesta más bonito
                return Ok(nuevaFicha);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la ficha: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        // POST: api/FichasIngreso/repuesto
        [HttpPost("repuesto")]
        public async Task<IActionResult> AgregarRepuesto(DetalleRepuestoCreateDto dto)
        {
            // 1. Buscar el producto y la ficha
            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            var ficha = await _context.FichasIngresos.FindAsync(dto.FichaId);

            if (producto == null) return NotFound("El producto no existe.");
            if (ficha == null) return NotFound("La ficha de ingreso no existe.");

            // 2. VALIDACIÓN DE STOCK
            if (producto.StockActual < dto.Cantidad)
            {
                return BadRequest($"Stock insuficiente. Solo quedan {producto.StockActual} unidades de {producto.Nombre}.");
            }

            // 3. REGISTRAR EL DETALLE (Ajustado a tus columnas reales)
            var detalle = new DetalleRepuesto
            {
                FichaId = dto.FichaId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                NumeroSerial = dto.NumeroSerial,
                // Eliminamos PrecioUnitario porque no existe en tu tabla
                DiasGarantia = 30
            };

            // 4. DESCONTAR STOCK
            producto.StockActual -= dto.Cantidad;
            producto.UltimaModificacion = DateTime.UtcNow;

            _context.DetalleRepuestos.Add(detalle);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = $"Repuesto '{producto.Nombre}' cargado a la ficha {dto.FichaId}.",
                    stockRestante = producto.StockActual
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        // POST: api/FichasIngreso/mano de obra
        [HttpPost("mano-obra")]
        public async Task<IActionResult> AgregarManoObra(DetalleManoObraCreateDto dto)
        {
            // 1. Buscamos el servicio en el catálogo y la ficha
            var servicio = await _context.CatalogoManoObras.FindAsync(dto.ServicioId);
            var ficha = await _context.FichasIngresos.FindAsync(dto.FichaId);

            if (servicio == null) return NotFound("El servicio no existe en el catálogo.");
            if (ficha == null) return NotFound("La ficha no existe.");

            // 2. Lógica del "Súper Poder" (Simplificada)
            decimal precioFinal;

            if (dto.PrecioCobrado > 0)
            {
                // Si el usuario mandó un precio manual, usamos ese
                precioFinal = dto.PrecioCobrado;
            }
            else
            {
                // Si no es anulable, simplemente asignamos el valor.
                precioFinal = servicio.PrecioFijo;
            }

            // 3. Mapeo a la entidad de la base de datos
            var detalle = new DetalleManoObra
            {
                FichaId = dto.FichaId,
                ServicioId = dto.ServicioId,
                PrecioCobrado = precioFinal
            };

            try
            {
                _context.DetalleManoObras.Add(detalle);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = $"Servicio '{servicio.Nombre}' cargado con éxito.",
                    precioAplicado = precioFinal,
                    esPrecioManual = dto.PrecioCobrado > 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        #endregion
        #region METODOS DE LECTURA GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FichaIngresoResponseDto>>> GetFichas()
        {
            return await _context.FichasIngresos
                .Include(f => f.Cliente)
                .Include(f => f.Bicicleta)
                .Include(f => f.Mecanico)
                .OrderByDescending(f => f.FechaEntrada) // Las más nuevas primero
                .Select(f => new FichaIngresoResponseDto
                {
                    FichaId = f.FichaId,
                    Estado = f.Estado,
                    FechaEntrada = f.FechaEntrada,
                    Observaciones = f.Observaciones,
                    // Mapeamos los nombres desde las tablas relacionadas
                    NombreCliente = f.Cliente != null ? f.Cliente.NombreCompleto : "Sin Cliente",
                    DatosBicicleta = f.Bicicleta != null ? $"{f.Bicicleta.Marca} ({f.Bicicleta.Color})" : "Sin Bici",
                    NombreMecanico = f.Mecanico != null ? f.Mecanico.NombreCompleto : "No asignado"
                })
                .ToListAsync();
        }
        [HttpGet("{id}/resumen")]
        public async Task<IActionResult> GetResumenFicha(int id)
        {
            // Buscamos la ficha con TODO su detalle vinculado
            var ficha = await _context.FichasIngresos
                .Include(f => f.Cliente)
                .Include(f => f.Bicicleta)
                .Include(f => f.DetalleRepuestos)
                    .ThenInclude(dr => dr.Producto) // Para ver el nombre del repuesto
                .Include(f => f.DetalleManoObras)
                    .ThenInclude(dm => dm.Servicio) // Para ver el nombre de la labor
                .FirstOrDefaultAsync(f => f.FichaId == id);

            if (ficha == null) return NotFound("Ficha no encontrada.");

            // Cálculo de totales
            // Sumamos: Cantidad * PrecioVenta (desde la tabla Producto)
            decimal totalRepuestos = ficha.DetalleRepuestos
                .Sum(r => (r.Cantidad) * (r.Producto?.PrecioVenta ?? 0));

            // Sumamos: PrecioCobrado (que ya guardamos en el detalle)
            decimal totalManoObra = ficha.DetalleManoObras
                .Sum(m => m.PrecioCobrado);

            return Ok(new
            {
                fichaId = ficha.FichaId,
                fecha = ficha.FechaEntrada,
                cliente = ficha.Cliente?.NombreCompleto,
                bicicleta = $"{ficha.Bicicleta?.Marca} {ficha.Bicicleta?.Modelo}",
                estado = ficha.Estado,

                // Desglose de Repuestos
                repuestos = ficha.DetalleRepuestos.Select(r => new {
                    descripcion = r.Producto?.Nombre,
                    cantidad = r.Cantidad,
                    precioUnitario = r.Producto?.PrecioVenta,
                    subtotal = (r.Cantidad) * (r.Producto?.PrecioVenta ?? 0)
                }),

                // Desglose de Mano de Obra
                servicios = ficha.DetalleManoObras.Select(m => new {
                    descripcion = m.Servicio?.Nombre,
                    precio = m.PrecioCobrado
                }),

                // Totales Finales
                resumenEconomico = new
                {
                    sumaRepuestos = totalRepuestos,
                    sumaManoObra = totalManoObra,
                    totalAPagar = totalRepuestos + totalManoObra
                }
            });
        }
        #endregion
        #region METODOS DE MODIFICACION PUT Y PATCH
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            var ficha = await _context.FichasIngresos.FindAsync(id);

            if (ficha == null) return NotFound("La ficha no existe.");

            // Lista de estados válidos (Regla de negocio)
            var estadosValidos = new List<string> { "Recibida", "En Proceso", "Finalizada", "Entregada", "Cancelada" };

            if (!estadosValidos.Contains(nuevoEstado))
            {
                return BadRequest($"Estado no válido. Use: {string.Join(", ", estadosValidos)}");
            }

            // Si el estado es "Finalizada", podríamos registrar automáticamente la FechaEntrega
            if (nuevoEstado == "Finalizada")
            {
                ficha.FechaEntrega = DateTime.UtcNow;
            }

            ficha.Estado = nuevoEstado;
            ficha.UltimaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"La ficha {id} ahora está en estado: {nuevoEstado}" });

        }
        #endregion
    }
}