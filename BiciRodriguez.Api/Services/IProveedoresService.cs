using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;
using Microsoft.EntityFrameworkCore;

public interface IProveedoresService
{
    Task<IEnumerable<ProveedorDto>> GetAllAsync();
}

// Implementación
public class ProveedoresService : IProveedoresService
{
    private readonly BiciContext _context;
    public ProveedoresService(BiciContext context) => _context = context;

    public async Task<IEnumerable<ProveedorDto>> GetAllAsync()
    {
        return await _context.Proveedores
            .Select(p => new ProveedorDto
            {
                ProveedorId = p.ProveedorId,
                Nombre = p.NombreEmpresa
            }).ToListAsync();
    }
}