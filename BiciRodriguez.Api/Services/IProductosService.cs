using BiciRodriguez.Api.DTOs;

namespace BiciRodriguez.Api.Services
{
    public interface IProductosService
    {
        Task<IEnumerable<ProductoDto>> GetAllAsync();
        Task<ProductoDto> CreateAsync(ProductoDto dto, int usuarioId);
        Task UpdateAsync(ProductoDto dto, int usuarioId); // Nuevo
        Task DeleteAsync(int id); // Nuevo
    }
}