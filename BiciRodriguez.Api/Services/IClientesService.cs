using BiciRodriguez.Api.DTOs;

namespace BiciRodriguez.Api.Services
{
    public interface IClientesService
    {
        Task<IEnumerable<ClienteDto>> GetAllAsync();
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<ClienteDto> CreateAsync(ClienteDto dto, int userId);
        Task<bool> UpdateAsync(int id, ClienteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}