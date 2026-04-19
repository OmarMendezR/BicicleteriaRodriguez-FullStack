namespace BiciRodriguez.Api.DTOs
{
    public class ProductoDto
    {
        public int ProductoId { get; set; }
        public int ProveedorId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioCompra { get; set; } 
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public int? StockMinimo { get; set; }
        public int? CategoriaId { get; set; }
        public string? NombreCategoria { get; set; }
        public bool Activo { get; set; } = true;
    }
}