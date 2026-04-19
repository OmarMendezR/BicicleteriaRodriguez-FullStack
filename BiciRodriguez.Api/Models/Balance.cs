using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class Balance
{
    public int BalanceId { get; set; }

    public DateOnly Fecha { get; set; }

    public string Tipo { get; set; } = null!;

    public decimal? IngresosServicios { get; set; }

    public decimal? IngresosRepuestos { get; set; }

    public decimal? TotalEgresos { get; set; }

    public decimal? UtilidadNeta { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public int? GeneradoPor { get; set; }
}
