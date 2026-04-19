namespace BiciRodriguez.Api.Models
{
    public class Proveedor
    {
        public int ProveedorId { get; set; }
        // SQL usa NombreEmpresa, no Nombre
        public string NombreEmpresa { get; set; } = string.Empty;
        public string? Nit { get; set; }
        public string? ContactoNombre { get; set; }
        public string? Telefono { get; set; }

        // Relación con Productos
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public virtual ICollection<Pedido> Pedidos { get; set; } = [];
    }
}