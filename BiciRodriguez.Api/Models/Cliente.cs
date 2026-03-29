using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class Cliente
{
    public int ClienteId { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? Direccion { get; set; }

    public bool? Activo { get; set; }

    public bool? AutorizaDatos { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public int? CreadoPorUsuarioId { get; set; }

    public DateTime? UltimaModificacion { get; set; }

    public virtual ICollection<Bicicleta> Bicicleta { get; set; } = new List<Bicicleta>();

    public virtual Usuario? CreadoPorUsuario { get; set; }

    public virtual ICollection<FichasIngreso> FichasIngresos { get; set; } = new List<FichasIngreso>();
}
