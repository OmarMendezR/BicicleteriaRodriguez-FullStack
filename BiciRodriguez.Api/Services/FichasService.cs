using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class FichasService : IFichasService
    {
        private readonly BiciContext _context;
        public FichasService(BiciContext context) => _context = context;
        #region SERVICIOS DE FICHAS DE INGRESO
        public async Task<IEnumerable<FichaIngresoResponseDto>> GetAllAsync()
        {
            var fichas = await _context.FichasIngresos
                .Include(f => f.Cliente)
                .Include(f => f.Bicicleta)
                .Include(f => f.Mecanico)
                .OrderByDescending(f => f.FechaEntrada)
                .ToListAsync();

            return fichas.Select(f => f.ToDto());
        }

        public async Task<FichaIngresoResponseDto> CreateFichaAsync(FichaIngresoCreateDto dto, int userId)
        {
            if (!await _context.Clientes.AnyAsync(c => c.ClienteId == dto.ClienteId))
                throw new Exception("Cliente no existe.");

            if (!await _context.Bicicletas.AnyAsync(b => b.BicicletaId == dto.BicicletaId))
                throw new Exception("Bicicleta no existe.");

            var nuevaFicha = dto.ToEntity(userId);
            _context.FichasIngresos.Add(nuevaFicha);
            await _context.SaveChangesAsync();

            var fichaConDatos = await _context.FichasIngresos
                .Include(f => f.Cliente)
                .Include(f => f.Bicicleta)
                .Include(f => f.Mecanico)
                .FirstAsync(f => f.FichaId == nuevaFicha.FichaId);

            return fichaConDatos.ToDto();
        }

        public async Task<FichaResumenDto?> GetResumenAsync(int id)
        {
            var ficha = await _context.FichasIngresos
                .Include(f => f.Cliente)
                .Include(f => f.DetalleRepuestos).ThenInclude(dr => dr.Producto)
                .Include(f => f.DetalleManoObras).ThenInclude(dm => dm.Servicio)
                .FirstOrDefaultAsync(f => f.FichaId == id);

            if (ficha == null) return null;

            var resumen = new FichaResumenDto
            {
                FichaId = ficha.FichaId,
                Cliente = ficha.Cliente?.NombreCompleto ?? "Cliente no registrado",
                Items = new List<DetalleItemDto>()
            };

            foreach (var r in ficha.DetalleRepuestos)
            {
                resumen.Items.Add(new DetalleItemDto
                {
                    Descripcion = $"[Repuesto] {r.Producto?.Nombre ?? "Producto no identificado"}",
                    Subtotal = r.Cantidad * r.PrecioVentaHistorico
                });
            }

            foreach (var m in ficha.DetalleManoObras)
            {
                resumen.Items.Add(new DetalleItemDto
                {
                    Descripcion = $"[Servicio] {m.Servicio?.Nombre ?? "Servicio Técnico"}",
                    Subtotal = m.PrecioCobrado
                });
            }

            resumen.TotalAPagar = resumen.Items.Sum(i => i.Subtotal);
            return resumen;
        }

        public async Task<bool> AddRepuestoAsync(DetalleRepuestoCreateDto dto)
        {
            // 1. Buscar el producto para validar stock y obtener precio
            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null) throw new Exception("Producto no encontrado.");

            if (producto.StockActual < dto.Cantidad)
                throw new Exception($"Stock insuficiente. Disponible: {producto.StockActual}");

            // 2. Restar del inventario
            producto.StockActual -= dto.Cantidad;
            producto.UltimaModificacion = DateTime.UtcNow;

            // 3. Registrar el detalle
            var detalle = new DetalleRepuesto
            {
                FichaId = dto.FichaId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                PrecioVentaHistorico = producto.PrecioVenta,
                NumeroSerial = dto.NumeroSerial ?? "N/A",
                DiasGarantia = dto.DiasGarantia ?? 30
            };

            _context.DetalleRepuestos.Add(detalle);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> AddManoObraAsync(DetalleManoObraCreateDto dto)
        {
            var detalle = new DetalleManoObra
            {
                FichaId = dto.FichaId,
                ServicioId = dto.ServicioId,
                PrecioCobrado = dto.PrecioCobrado
            };
            _context.DetalleManoObras.Add(detalle);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateEstadoAsync(int id, string nuevoEstado)
        {
            var ficha = await _context.FichasIngresos.FindAsync(id);
            if (ficha == null) return false;

            var estadosValidos = new[] { "Recibida", "En Proceso", "Finalizada", "Entregada", "Cancelada" };
            if (!estadosValidos.Contains(nuevoEstado)) throw new Exception("Estado no válido.");

            ficha.Estado = nuevoEstado;
            ficha.UltimaModificacion = DateTime.UtcNow;
            if (nuevoEstado == "Finalizada") ficha.FechaEntrega = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
#endregion

        #region SERVICIOS DE CAJA
        public async Task<decimal> GetIngresosDelDiaAsync(DateTime fecha)
        {
            // Suma de repuestos usados en fichas hoy
            var totalRepuestos = await _context.DetalleRepuestos
                .Where(dr => dr.Ficha.FechaEntrada.Value.Date == fecha.Date)
                .SumAsync(dr => dr.Cantidad * dr.PrecioVentaHistorico);

            // Suma de mano de obra cobrada hoy
            var totalManoObra = await _context.DetalleManoObras
                .Where(dm => dm.Ficha.FechaEntrada.Value.Date == fecha.Date)
                .SumAsync(dm => dm.PrecioCobrado);

            return totalRepuestos + totalManoObra;
        }
        #endregion
    }
}