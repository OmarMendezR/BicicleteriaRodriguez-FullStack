using BiciRodriguez.Api.DTOs;

namespace BiciRodriguez.Api.Services
{
    public interface IProductosService
    {
        Task<IEnumerable<ProductoDto>> GetAllAsync();
        Task<ProductoDto> CreateAsync(ProductoDto dto, int userId);
    }
}