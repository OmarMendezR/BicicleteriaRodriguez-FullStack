using System;
using System.Collections.Generic;

namespace BiciRodriguez.Api.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RolId { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Bicicleta> Bicicleta { get; set; } = new List<Bicicleta>();

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<FichasIngreso> FichasIngresoCreadoPorUsuarios { get; set; } = new List<FichasIngreso>();

    public virtual ICollection<FichasIngreso> FichasIngresoMecanicos { get; set; } = new List<FichasIngreso>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual Role Rol { get; set; } = null!;
}
