namespace BiciRodriguez.Api.DTOs
{
    public class SugerenciaPedidoDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public int CantidadSugerida { get; set; } 
    }
}