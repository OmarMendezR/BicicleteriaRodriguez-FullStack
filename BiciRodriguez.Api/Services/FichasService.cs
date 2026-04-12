using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class FichasService : IFichasService
    {
        private readonly BiciContext _context;

        public FichasService(BiciContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FichaIngresoResponseDto>> GetAllAsync()
        {
            return await _context.FichasIngresos
                .Include(f => f.Cliente).Include(f => f.Bicicleta).Include(f => f.Mecanico)
                .OrderByDescending(f => f.FechaEntrada)
                .Select(f => new FichaIngresoResponseDto
                {
                    FichaId = f.FichaId,
                    Estado = f.Estado,
                    FechaEntrada = f.FechaEntrada,
                    Observaciones = f.Observaciones,
                    NombreCliente = f.Cliente != null ? f.Cliente.NombreCompleto : "Sin Cliente",
                    DatosBicicleta = f.Bicicleta != null ? $"{f.Bicicleta.Marca} ({f.Bicicleta.Color})" : "Sin Bici",
                    NombreMecanico = f.Mecanico != null ? f.Mecanico.NombreCompleto : "No asignado"
                }).ToListAsync();
        }

        public async Task<FichasIngreso> CreateFichaAsync(FichaIngresoCreateDto dto, int userId)
        {
            var bicicleta = await _context.Bicicletas
                .FirstOrDefaultAsync(b => b.BicicletaId == dto.BicicletaId && b.ClienteId == dto.ClienteId);

            if (bicicleta == null) throw new Exception("La bicicleta no pertenece al cliente.");

            var nuevaFicha = new FichasIngreso
            {
                ClienteId = dto.ClienteId,
                BicicletaId = dto.BicicletaId,
                MecanicoId = dto.MecanicoId,
                Observaciones = dto.Observaciones,
                Estado = dto.Estado,
                FechaEntrada = DateTime.UtcNow,
                UltimaModificacion = DateTime.UtcNow,
                CreadoPorUsuarioId = userId
            };

            _context.FichasIngresos.Add(nuevaFicha);
            await _context.SaveChangesAsync();
            return nuevaFicha;
        }

        public async Task<object> AddRepuestoAsync(DetalleRepuestoCreateDto dto)
        {
            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            var ficha = await _context.FichasIngresos.FindAsync(dto.FichaId);

            if (producto == null || ficha == null) throw new Exception("Producto o Ficha no encontrados.");
            if (producto.StockActual < dto.Cantidad) throw new Exception($"Stock insuficiente. Quedan {producto.StockActual}.");

            var detalle = new DetalleRepuesto
            {
                FichaId = dto.FichaId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                NumeroSerial = dto.NumeroSerial,
                DiasGarantia = 30
            };

            producto.StockActual -= dto.Cantidad;
            _context.DetalleRepuestos.Add(detalle);
            await _context.SaveChangesAsync();

            return new { mensaje = "Repuesto cargado", stockRestante = producto.StockActual };
        }

        public async Task<object> AddManoObraAsync(DetalleManoObraCreateDto dto)
        {
            var servicio = await _context.CatalogoManoObras.FindAsync(dto.ServicioId);
            if (servicio == null) throw new Exception("Servicio no encontrado.");

            decimal precioFinal = dto.PrecioCobrado > 0 ? dto.PrecioCobrado : servicio.PrecioFijo;

            var detalle = new DetalleManoObra
            {
                FichaId = dto.FichaId,
                ServicioId = dto.ServicioId,
                PrecioCobrado = precioFinal
            };

            _context.DetalleManoObras.Add(detalle);
            await _context.SaveChangesAsync();

            return new { mensaje = "Servicio cargado", precioAplicado = precioFinal };
        }

        public async Task<object?> GetResumenAsync(int id)
        {
            var ficha = await _context.FichasIngresos
                .Include(f => f.Cliente).Include(f => f.Bicicleta)
                .Include(f => f.DetalleRepuestos).ThenInclude(dr => dr.Producto)
                .Include(f => f.DetalleManoObras).ThenInclude(dm => dm.Servicio)
                .FirstOrDefaultAsync(f => f.FichaId == id);

            if (ficha == null) return null;

            decimal totalRepuestos = ficha.DetalleRepuestos.Sum(r => r.Cantidad * (r.Producto?.PrecioVenta ?? 0));
            decimal totalManoObra = ficha.DetalleManoObras.Sum(m => m.PrecioCobrado);

            return new
            {
                fichaId = ficha.FichaId,
                cliente = ficha.Cliente?.NombreCompleto,
                totalAPagar = totalRepuestos + totalManoObra,
                repuestos = ficha.DetalleRepuestos.Select(r => new { desc = r.Producto?.Nombre, sub = r.Cantidad * r.Producto?.PrecioVenta }),
                servicios = ficha.DetalleManoObras.Select(m => new { desc = m.Servicio?.Nombre, precio = m.PrecioCobrado })
            };
        }

        public async Task<bool> UpdateEstadoAsync(int id, string nuevoEstado)
        {
            var ficha = await _context.FichasIngresos.FindAsync(id);
            if (ficha == null) return false;

            var estadosValidos = new List<string> { "Recibida", "En Proceso", "Finalizada", "Entregada", "Cancelada" };
            if (!estadosValidos.Contains(nuevoEstado)) throw new Exception("Estado inválido.");

            if (nuevoEstado == "Finalizada") ficha.FechaEntrega = DateTime.UtcNow;

            ficha.Estado = nuevoEstado;
            ficha.UltimaModificacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}