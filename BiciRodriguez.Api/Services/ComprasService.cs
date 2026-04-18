using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Interfaces;
using BiciRodriguez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class ComprasService : IComprasService
    {
        private readonly BiciContext _context;

        public ComprasService(BiciContext context)
        {
            _context = context;
        }

        public async Task<bool> RecibirPedidoAsync(int pedidoId)
        {
            // 1. Cargamos el pedido con sus detalles y los datos del producto
            // Usamos .Include para traer la "información relacionada"
            var pedido = await _context.Pedidos
                .Include(p => p.DetallePedidos)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

            // Validación de seguridad: Si no existe o ya está recibido, cancelamos
            if (pedido == null || pedido.Estado == "Recibido")
            {
                return false;
            }

            // 2. Iniciamos una Transacción para asegurar que si algo falla, 
            // no se actualice el stock a medias (Integridad de Datos ISO 27001)
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var detalle in pedido.DetallePedidos)
                {
                    if (detalle.Producto != null)
                    {
                        // Actualizamos el StockActual (manejamos el null con ?? 0)
                        detalle.Producto.StockActual = (detalle.Producto.StockActual ?? 0) + detalle.Cantidad;

                        // Actualizamos el Precio de Compra con el valor que llegó en este pedido
                        detalle.Producto.PrecioCompra = detalle.PrecioCompra;

                        // Actualizamos la fecha de última modificación para auditoría
                        detalle.Producto.UltimaModificacion = DateTime.Now;
                    }
                }

                // 3. Cambiamos el estado del pedido
                pedido.Estado = "Recibido";

                // 4. Guardamos todos los cambios en la DB
                await _context.SaveChangesAsync();

                // Confirmamos la transacción
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                // Si algo sale mal (ej. se va la luz), revertimos todo
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<IEnumerable<SugerenciaPedidoDto>> GetProductosEnAlertaAsync()
        {
            return await _context.Productos
                .Where(p => p.Activo == true && p.StockActual <= p.StockMinimo)
                .Select(p => new SugerenciaPedidoDto
                {
                    ProductoId = p.ProductoId,
                    Nombre = p.Nombre,
                    StockActual = p.StockActual ?? 0,
                    StockMinimo = p.StockMinimo ?? 0,
                    // Lógica simple: reponer hasta doblar el mínimo
                    CantidadSugerida = ((p.StockMinimo ?? 5) * 2) - (p.StockActual ?? 0)
                })
                .ToListAsync();
        }

        public async Task<int> CrearPedidoBorradorAsync(List<SugerenciaPedidoDto> items)
        {
            if (items == null || !items.Any()) return 0;

            // 1. Creamos la cabecera del pedido (Sin proveedor, tal como querías)
            var nuevoPedido = new Pedido
            {
                FechaCreacion = DateTime.Now,
                Estado = "Pendiente",
                ProveedorId = null, // Es opcional para ir de compras al centro
                TotalPedido = items.Sum(x => 0) // El total se llenará cuando sepamos los precios reales
            };

            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync(); // Guardamos para obtener el PedidoID

            // 2. Creamos los detalles
            foreach (var item in items)
            {
                var detalle = new DetallePedido
                {
                    PedidoId = nuevoPedido.PedidoId,
                    ProductoId = item.ProductoId,
                    Cantidad = item.CantidadSugerida,
                    PrecioCompra = 0 // Se actualizará cuando el admin regrese con la factura
                };
                _context.DetallePedidos.Add(detalle);
            }

            await _context.SaveChangesAsync();
            return nuevoPedido.PedidoId;
        }
    }
}