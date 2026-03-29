using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class FichasIngreso
{
    public int FichaId { get; set; }

    public int? ClienteId { get; set; }

    public int? MecanicoId { get; set; }

    public DateTime? FechaEntrada { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public string? Estado { get; set; }

    public string? Observaciones { get; set; }

    public DateTime? UltimaModificacion { get; set; }

    public int? BicicletaId { get; set; }

    public int? CreadoPorUsuarioId { get; set; }

    public int? ModificadoPorUsuarioId { get; set; }

    public virtual Bicicleta? Bicicleta { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Usuario? CreadoPorUsuario { get; set; }

    public virtual ICollection<DetalleManoObra> DetalleManoObras { get; set; } = new List<DetalleManoObra>();

    public virtual ICollection<DetalleRepuesto> DetalleRepuestos { get; set; } = new List<DetalleRepuesto>();

    public virtual Usuario? Mecanico { get; set; }
}
