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
                    // AGREGA ESTA LÍNEA:
                    StockMinimo = p.StockMinimo ?? 5,
                    CategoriaId = p.CategoriaId,
                    NombreCategoria = p.Categoria != null ? p.Categoria.Nombre : "Sin Categoría",
                    ProveedorId = p.ProveedorId ?? 0,
                    Activo = p.Activo ?? false
                })
                .ToListAsync();
        }

        public async Task<ProductoDto> CreateAsync(ProductoDto dto, int userId)
        {
            ValidarReglasNegocio(dto);

            var nuevoProducto = new Producto
            {
                Nombre = dto.Nombre,
                PrecioCompra = dto.PrecioCompra,
                PrecioVenta = dto.PrecioVenta,
                StockActual = dto.Stock,
                StockMinimo = dto.StockMinimo,
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

        public async Task UpdateAsync(ProductoDto dto, int usuarioId)
        {
            ValidarReglasNegocio(dto);

            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null) throw new Exception("Producto no encontrado");

            producto.Nombre = dto.Nombre;
            producto.PrecioCompra = dto.PrecioCompra;
            producto.PrecioVenta = dto.PrecioVenta;
            producto.StockActual = dto.Stock;
            producto.StockMinimo = dto.StockMinimo;
            producto.CategoriaId = (dto.CategoriaId <= 0) ? null : dto.CategoriaId;
            producto.ProveedorId = dto.ProveedorId;
            producto.Activo = dto.Activo;
            producto.UltimaModificacion = DateTime.UtcNow;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) throw new Exception("El producto no existe.");

            // Eliminación Lógica
            producto.Activo = false;
            producto.UltimaModificacion = DateTime.UtcNow;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        private void ValidarReglasNegocio(ProductoDto dto)
        {
            if (dto.Stock < 0)
                throw new Exception("El stock no puede ser negativo.");

            if (dto.PrecioVenta <= dto.PrecioCompra)
                throw new Exception("Rentabilidad insuficiente: El precio de venta debe ser mayor al de compra.");
        }
    }
}