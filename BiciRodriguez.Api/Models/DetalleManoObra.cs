using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class DetalleManoObra
{
    public int DetalleServicioId { get; set; }

    public int? FichaId { get; set; }

    public int? ServicioId { get; set; }

    public decimal PrecioCobrado { get; set; }

    public virtual FichasIngreso? Ficha { get; set; }

    public virtual CatalogoManoObra? Servicio { get; set; }
}
