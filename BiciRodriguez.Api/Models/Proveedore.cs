using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class Proveedore
{
    public int ProveedorId { get; set; }

    public string NombreEmpresa { get; set; } = null!;

    public string? Nit { get; set; }

    public string? ContactoNombre { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
