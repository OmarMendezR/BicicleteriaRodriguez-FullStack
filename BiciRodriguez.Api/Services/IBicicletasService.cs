using BiciRodriguez.Api.DTOs;

namespace BiciRodriguez.Api.Services
{
    public interface IBicicletasService
    {
        Task<IEnumerable<BicicletaResponseDto>> GetAllAsync();
        Task<BicicletaResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<BicicletaResponseDto>> GetByClienteIdAsync(int clienteId);
        Task<BicicletaResponseDto> CreateAsync(BicicletaResponseDto biciDto, int userId);
        Task<bool> UpdateAsync(int id, BicicletaResponseDto biciDto);
        Task<bool> DeleteAsync(int id);
    }
}