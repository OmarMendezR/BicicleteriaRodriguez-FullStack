using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class Pedido
{
    public int PedidoId { get; set; }

    public int? ProveedorId { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public decimal? TotalPedido { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual Proveedore? Proveedor { get; set; }
}
