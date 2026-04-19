using BiciRodriguez.Api.Interfaces;
using BiciRodriguez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class BalancesService : IBalancesService
    {
        private readonly BiciContext _context;

        public BalancesService(BiciContext context)
        {
            _context = context;
        }

        public async Task<bool> GenerarCierreDiarioAsync(int? usuarioId)
        {
            var hoy = DateTime.Today;
            // IMPORTANTE: Convertimos a DateOnly para que coincida con la propiedad del modelo Balance
            var hoyDateOnly = DateOnly.FromDateTime(hoy);

            // 1. Ingresos por Mano de Obra (Manejando DateTime? con .Value)
            var ingresosServicios = await _context.DetalleManoObras
                .Where(m => m.Ficha.Estado == "Terminado" &&
                            m.Ficha.FechaEntrada.HasValue &&
                            m.Ficha.FechaEntrada.Value.Date == hoy)
                .SumAsync(m => (decimal?)m.PrecioCobrado) ?? 0;

            // 2. Ingresos por Repuestos
            var ingresosRepuestos = await _context.DetalleRepuestos
                .Where(r => r.Ficha.Estado == "Terminado" &&
                            r.Ficha.FechaEntrada.HasValue &&
                            r.Ficha.FechaEntrada.Value.Date == hoy)
                .SumAsync(r => (decimal?)(r.Cantidad * r.PrecioVentaHistorico)) ?? 0;

            // 3. Egresos (Compras)
            var totalEgresos = await _context.Pedidos
                .Where(p => p.Estado == "Recibido" &&
                            p.FechaCreacion.HasValue &&
                            p.FechaCreacion.Value.Date == hoy)
                .SumAsync(p => (decimal?)p.TotalPedido) ?? 0;

            // 4. Crear el registro
            var nuevoBalance = new Balance
            {
                Fecha = hoyDateOnly, // Ahora sí coinciden los tipos
                Tipo = "Diario",
                IngresosServicios = ingresosServicios,
                IngresosRepuestos = ingresosRepuestos,
                TotalEgresos = totalEgresos,
                GeneradoPor = usuarioId,
                FechaCreacion = DateTime.Now
            };

            _context.Balances.Add(nuevoBalance);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Balance>> GetHistorialAsync()
        {
            return await _context.Balances
                .OrderByDescending(b => b.Fecha)
                .ToListAsync();
        }

        public async Task<bool> GenerarCierreMensualAsync(int anio, int mes, int? usuarioId)
        {
            // Lógica para consolidado mensual (puedes implementarla luego)
            return await Task.FromResult(true);
        }
    }
}