namespace BiciRodriguez.Api.DTOs
{
    public class DetalleManoObraCreateDto
    {
        public int FichaId { get; set; }
        public int ServicioId { get; set; }
        public decimal PrecioCobrado { get; set; }
    }
}