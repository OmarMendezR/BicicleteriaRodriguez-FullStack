using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class DetalleRepuesto
{
    public int DetalleInsumoId { get; set; }

    public int? FichaId { get; set; }

    public int? ProductoId { get; set; }

    public int Cantidad { get; set; }

    public string? NumeroSerial { get; set; }

    public int? DiasGarantia { get; set; }

    public virtual FichasIngreso? Ficha { get; set; }

    public virtual Producto? Producto { get; set; }
}
