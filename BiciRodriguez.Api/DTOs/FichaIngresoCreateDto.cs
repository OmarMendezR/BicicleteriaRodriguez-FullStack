namespace BiciRodriguez.Api.DTOs
{
    public class FichaIngresoCreateDto
    {
        public int ClienteId { get; set; }
        public int BicicletaId { get; set; }
        public int MecanicoId { get; set; }
        public string? Observaciones { get; set; }
        public string Estado { get; set; } = "Recibida";
    }
}