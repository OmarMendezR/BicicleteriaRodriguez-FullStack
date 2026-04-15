namespace BiciRodriguez.Api.DTOs
{
    public class DetalleRepuestoCreateDto
    {
        public int FichaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? NumeroSerial { get; set; }
        public int? DiasGarantia { get; set; } 
    }
}