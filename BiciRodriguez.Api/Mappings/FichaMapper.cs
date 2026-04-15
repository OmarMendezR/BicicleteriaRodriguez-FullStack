using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;

namespace BiciRodriguez.Api.Mappings
{
    public static class FichaMapper
    {
        public static FichaIngresoResponseDto ToDto(this FichasIngreso f)
        {
            if (f == null) return null!;

            return new FichaIngresoResponseDto
            {
                FichaId = f.FichaId,
                Estado = f.Estado,
                FechaEntrada = f.FechaEntrada,
                Observaciones = f.Observaciones,
                NombreCliente = f.Cliente?.NombreCompleto ?? "Sin Cliente",
                DatosBicicleta = f.Bicicleta != null
                    ? $"{f.Bicicleta.Marca} ({f.Bicicleta.Color})"
                    : "Sin Bici",
                NombreMecanico = f.Mecanico?.NombreCompleto ?? "No asignado"
            };
        }

        public static FichasIngreso ToEntity(this FichaIngresoCreateDto dto, int userId)
        {
            return new FichasIngreso
            {
                ClienteId = dto.ClienteId,
                BicicletaId = dto.BicicletaId,
                MecanicoId = dto.MecanicoId,
                Observaciones = dto.Observaciones,
                Estado = "Recibida", // Estado inicial por defecto
                FechaEntrada = DateTime.UtcNow,
                UltimaModificacion = DateTime.UtcNow,
                CreadoPorUsuarioId = userId
            };
        }
    }
}