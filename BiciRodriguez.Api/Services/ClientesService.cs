using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class ClientesService : IClientesService
    {
        private readonly BiciContext _context;

        public ClientesService(BiciContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteDto>> GetAllAsync()
        {
            return await _context.Clientes
                .Select(c => new ClienteDto
                {
                    ClienteId = c.ClienteId,
                    Nombre = c.NombreCompleto,
                    Telefono = c.Telefono ?? string.Empty,
                    Correo = c.Email ?? string.Empty,
                    Direccion = c.Direccion ?? string.Empty
                }).ToListAsync();
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var c = await _context.Clientes.FindAsync(id);
            if (c == null) return null;

            return new ClienteDto
            {
                ClienteId = c.ClienteId,
                Nombre = c.NombreCompleto,
                Telefono = c.Telefono ?? string.Empty,
                Correo = c.Email ?? string.Empty,
                Direccion = c.Direccion ?? string.Empty
            };
        }

        public async Task<ClienteDto> CreateAsync(ClienteDto dto, int userId)
        {
            var nuevoCliente = new Cliente
            {
                NombreCompleto = $"{dto.Nombre} {dto.Apellido}".Trim(),
                Telefono = dto.Telefono,
                Email = dto.Correo,
                Direccion = dto.Direccion,
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                AutorizaDatos = true,
                CreadoPorUsuarioId = userId
            };

            _context.Clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            dto.ClienteId = nuevoCliente.ClienteId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, ClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            cliente.NombreCompleto = $"{dto.Nombre} {dto.Apellido}".Trim();
            cliente.Telefono = dto.Telefono;
            cliente.Email = dto.Correo;
            cliente.Direccion = dto.Direccion;
            cliente.UltimaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            // REGLA DE NEGOCIO: No borrar clientes con bicicletas vinculadas
            var tieneBicis = await _context.Bicicletas.AnyAsync(b => b.ClienteId == id);
            if (tieneBicis)
            {
                throw new Exception("No se puede eliminar: Este cliente tiene bicicletas registradas.");
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}