using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class ProductosService : IProductosService
    {
        private readonly BiciContext _context;

        public ProductosService(BiciContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllAsync()
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

        public async Task<ProductoDto> CreateAsync(ProductoDto dto, int userId)
        {
            // VALIDACIONES DE NEGOCIO
            if (dto.Stock < 0)
                throw new Exception("El stock inicial no puede ser negativo.");

            if (dto.PrecioVenta <= dto.PrecioCompra)
                throw new Exception("Rentabilidad insuficiente: El precio de venta debe superar al de compra.");

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
                CreadoPorUsuarioId = userId,
                ProveedorId = dto.ProveedorId
            };

            _context.Productos.Add(nuevoProducto);
            await _context.SaveChangesAsync();

            dto.ProductoId = nuevoProducto.ProductoId;
            return dto;
        }
    }
}