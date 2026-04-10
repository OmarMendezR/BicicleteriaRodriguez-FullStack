namespace BiciRodriguez.Api.DTOs
{
    public class FichaIngresoCreateDto
    {
        // Los IDs necesarios para vincular la operación
        public int ClienteId { get; set; }
        public int BicicletaId { get; set; }
        public int MecanicoId { get; set; }

        // Información de entrada
        public string? Observaciones { get; set; }

        // El estado lo podemos manejar por defecto como "Recibida" 
        // pero lo dejamos aquí por si se quiere cambiar
        public string Estado { get; set; } = "Recibida";
    }
}