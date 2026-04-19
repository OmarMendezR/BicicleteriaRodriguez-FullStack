using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal PrecioVenta { get; set; }

    public int? StockActual { get; set; }

    public int? StockMinimo { get; set; }

    public int? CategoriaId { get; set; }

    public int? ProveedorId { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activo { get; set; }

    public decimal PrecioCompra { get; set; }

    public int? CreadoPorUsuarioId { get; set; }

    public DateTime? UltimaModificacion { get; set; }

    public virtual Categoria? Categoria { get; set; }

    public virtual Usuario? CreadoPorUsuario { get; set; }

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual ICollection<DetalleRepuesto> DetalleRepuestos { get; set; } = new List<DetalleRepuesto>();

    public virtual Proveedor? Proveedor { get; set; }
}
