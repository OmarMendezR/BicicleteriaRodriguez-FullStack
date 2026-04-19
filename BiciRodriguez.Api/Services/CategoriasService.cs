using Microsoft.EntityFrameworkCore;
using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;

public class CategoriasService : ICategoriasService
{
    private readonly BiciContext _context;
    public CategoriasService(BiciContext context) => _context = context;

    public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
    {
        return await _context.Categorias
            .Select(c => new CategoriaDto
            {
                CategoriaId = c.CategoriaId,
                Nombre = c.Nombre
            }).ToListAsync();
    }
}