namespace BiciRodriguez.Api.DTOs
{
    public class FichaIngresoResponseDto
    {
        public int FichaId { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaEntrada { get; set; }
        public string? Observaciones { get; set; }

        // Información "traducida" para humanos
        public string? NombreCliente { get; set; }
        public string? DatosBicicleta { get; set; } // Ej: "GW Raven - Color Negro"
        public string? NombreMecanico { get; set; }
    }
}