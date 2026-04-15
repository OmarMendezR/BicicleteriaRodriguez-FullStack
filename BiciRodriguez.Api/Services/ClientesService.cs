using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class ClientesService : IClientesService
    {
        private readonly BiciContext _context;

        public ClientesService(BiciContext context) => _context = context;

        public async Task<IEnumerable<ClienteDto>> GetAllAsync()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return clientes.Select(c => c.ToDto());
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var c = await _context.Clientes.FindAsync(id);
            return c?.ToDto();
        }

        public async Task<ClienteDto> CreateAsync(ClienteDto dto, int userId)
        {
            var nuevoCliente = dto.ToEntity(userId);
            _context.Clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            return nuevoCliente.ToDto();
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

            if (await _context.Bicicletas.AnyAsync(b => b.ClienteId == id))
                throw new Exception("No se puede eliminar: Este cliente tiene bicicletas registradas.");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}