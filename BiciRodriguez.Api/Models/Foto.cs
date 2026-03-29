using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class Foto
{
    public int FotoId { get; set; }

    public int RelacionId { get; set; }

    public string TipoEntidad { get; set; } = null!;

    public string RutaArchivo { get; set; } = null!;

    public bool? EsPrincipal { get; set; }

    public DateTime? FechaRegistro { get; set; }
}
