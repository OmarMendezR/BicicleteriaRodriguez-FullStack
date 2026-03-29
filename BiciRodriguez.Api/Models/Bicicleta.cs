using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class Bicicleta
{
    public int BicicletaId { get; set; }

    public int? ClienteId { get; set; }

    public string Marca { get; set; } = null!;

    public string? Modelo { get; set; }

    public string NumeroMarco { get; set; } = null!;

    public string? Color { get; set; }

    public string? TipoBicicleta { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? CreadoPorUsuarioId { get; set; }

    public DateTime? UltimaModificacion { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Usuario? CreadoPorUsuario { get; set; }

    public virtual ICollection<FichasIngreso> FichasIngresos { get; set; } = new List<FichasIngreso>();
}
