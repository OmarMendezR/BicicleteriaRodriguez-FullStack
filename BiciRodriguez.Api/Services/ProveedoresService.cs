using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class ProveedoresService : IProveedoresService
    {
        private readonly BiciContext _context;

        public ProveedoresService(BiciContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProveedorDto>> GetAllAsync()
        {
            // Usamos _context.Proveedor (singular) como está en tu BiciContext
            return await _context.Proveedores
                .Select(p => new ProveedorDto
                {
                    ProveedorId = p.ProveedorId,
                    Nombre = p.NombreEmpresa
                })
                .ToListAsync<ProveedorDto>(); // Especificar el DTO aquí mata el error de inferencia
        }
    }
}