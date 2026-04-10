namespace BiciRodriguez.Api.DTOs
{
    public class DetalleRepuestoCreateDto
    {
        public int FichaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? NumeroSerial { get; set; } // Opcional, por si es un marco o componente con serie
    }
}