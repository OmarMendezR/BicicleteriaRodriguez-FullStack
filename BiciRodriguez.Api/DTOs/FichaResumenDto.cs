namespace BiciRodriguez.Api.DTOs
{
    public class FichaResumenDto
    {
        public int FichaId { get; set; }
        public string? Cliente { get; set; }
        public decimal TotalAPagar { get; set; }
        public List<DetalleItemDto> Items { get; set; } = new();
    }

    public class DetalleItemDto
    {
        public string? Descripcion { get; set; }
        public decimal Subtotal { get; set; }
    }
}