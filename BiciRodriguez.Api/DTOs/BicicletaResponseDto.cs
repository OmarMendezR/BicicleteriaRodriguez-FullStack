namespace BiciRodriguez.Api.DTOs
{
    public class BicicletaResponseDto
    {
        public int BicicletaId { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string NumeroMarco { get; set; } = string.Empty;
        public int? ClienteId { get; set; }
        public string Color { get; set; } = "Sin especificar";
        public string TipoBicicleta { get; set; } = "MTB";
    }
}