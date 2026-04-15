using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;

namespace BiciRodriguez.Api.Mappings
{
    public static class ClienteMapper
    {
        public static ClienteDto ToDto(this Cliente c)
        {
            if (c == null) return null!;

            // Dividimos el nombre completo para el DTO (asumiendo formato "Nombre Apellido")
            var nombres = c.NombreCompleto.Split(' ', 2);

            return new ClienteDto
            {
                ClienteId = c.ClienteId,
                Nombre = nombres.Length > 0 ? nombres[0] : "",
                Apellido = nombres.Length > 1 ? nombres[1] : "",
                Telefono = c.Telefono ?? string.Empty,
                Correo = c.Email ?? string.Empty,
                Direccion = c.Direccion ?? string.Empty
            };
        }

        public static Cliente ToEntity(this ClienteDto dto, int userId)
        {
            return new Cliente
            {
                NombreCompleto = $"{dto.Nombre} {dto.Apellido}".Trim(),
                Telefono = dto.Telefono,
                Email = dto.Correo,
                Direccion = dto.Direccion,
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                AutorizaDatos = true,
                CreadoPorUsuarioId = userId,
                UltimaModificacion = DateTime.UtcNow
            };
        }
    }
}