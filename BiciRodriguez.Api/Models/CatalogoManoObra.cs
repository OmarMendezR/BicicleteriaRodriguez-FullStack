using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class CatalogoManoObra
{
    public int ServicioId { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal PrecioFijo { get; set; }

    public string? DuracionEstimada { get; set; }

    public virtual ICollection<DetalleManoObra> DetalleManoObras { get; set; } = new List<DetalleManoObra>();
}
