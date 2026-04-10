using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly BiciContext _context;

        public ProductosController(BiciContext context)
        {
            _context = context;
        }
        #region METODOS GET DE LECTURA
        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Select(p => new ProductoDto
                {
                    ProductoId = p.ProductoId,
                    Nombre = p.Nombre,
                    PrecioCompra = p.PrecioCompra,
                    PrecioVenta = p.PrecioVenta,
                    Stock = p.StockActual ?? 0,
                    CategoriaId = p.CategoriaId,
                    NombreCategoria = p.Categoria != null ? p.Categoria.Nombre : "Sin Categoría",
                    ProveedorId = p.ProveedorId ?? 0,
                    Activo = p.Activo ?? false
                })
                .ToListAsync();
        }
        #endregion
        #region METODOS POST DE CREACION
        // POST: api/Productos
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> PostProducto(ProductoDto dto)
        {
            // 1. VALIDACIONES DE NEGOCIO (Las 3 sugerencias)
            if (dto.Stock < 0) return BadRequest("El stock inicial no puede ser negativo.");
            if (dto.PrecioVenta <= dto.PrecioCompra) return BadRequest("Rentabilidad insuficiente: El precio de venta debe superar al de compra.");

            // 2. IDENTIDAD
            var userIdClaim = User.FindFirst("id")?.Value ?? "1";
            int currentUserId = int.Parse(userIdClaim);

            // 3. MAPEO DE ENTIDAD (Ajustado al BiciContext)
            var nuevoProducto = new Producto
            {
                Nombre = dto.Nombre,
                PrecioCompra = dto.PrecioCompra,
                PrecioVenta = dto.PrecioVenta,
                StockActual = dto.Stock,
                CategoriaId = (dto.CategoriaId <= 0) ? null : dto.CategoriaId,

                Activo = dto.Activo,
                FechaRegistro = DateTime.UtcNow,
                UltimaModificacion = DateTime.UtcNow,
                CreadoPorUsuarioId = currentUserId,

                ProveedorId = dto.ProveedorId
            };

            try
            {
                _context.Productos.Add(nuevoProducto);
                await _context.SaveChangesAsync();

                dto.ProductoId = nuevoProducto.ProductoId;
                return CreatedAtAction(nameof(GetProductos), new { id = dto.ProductoId }, dto);
            }
            catch (DbUpdateException ex)
            {
                // Esto atrapará errores de FK y nos dirá el detalle exacto
                return StatusCode(500, $"Error de base de datos: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        #endregion
        #region METODOS PUT DE ACTUALIZACION

        #endregion
        #region METODOS DELETE DE ELIMINACION

        #endregion
    }
}