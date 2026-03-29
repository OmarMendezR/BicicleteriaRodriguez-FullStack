using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class DetallePedido
{
    public int DetalleId { get; set; }

    public int? PedidoId { get; set; }

    public int? ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioCompra { get; set; }

    public virtual Pedido? Pedido { get; set; }

    public virtual Producto? Producto { get; set; }
}
