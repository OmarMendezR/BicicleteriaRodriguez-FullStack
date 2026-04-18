using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;

namespace BiciRodriguez.Api.Mappings
{
    public static class BicicletaMapper
    {
        // Extension Method: Convierte de Entidad a DTO
        public static BicicletaResponseDto ToDto(this Bicicleta b)
        {
            if (b == null) return null!;

            return new BicicletaResponseDto
            {
                BicicletaId = b.BicicletaId,
                Marca = b.Marca,
                NumeroMarco = b.NumeroMarco,
                ClienteId = b.ClienteId,
                Color = b.Color ?? "N/A",
                TipoBicicleta = b.TipoBicicleta ?? "General"
            };
        }

        // Extension Method: Convierte de DTO a Entidad (para crear)
        public static Bicicleta ToEntity(this BicicletaResponseDto dto, int userId)
        {
            return new Bicicleta
            {
                Marca = dto.Marca,
                NumeroMarco = dto.NumeroMarco,
                ClienteId = dto.ClienteId == 0 ? null : dto.ClienteId,
                Color = dto.Color,
                TipoBicicleta = dto.TipoBicicleta,
                CreadoPorUsuarioId = userId,
                FechaRegistro = DateTime.UtcNow,
                UltimaModificacion = DateTime.UtcNow,
                Modelo = "N/A" // Valor por defecto
            };
        }
    }
}