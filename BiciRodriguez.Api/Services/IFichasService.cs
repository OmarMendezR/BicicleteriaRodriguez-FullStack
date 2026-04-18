using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;

namespace BiciRodriguez.Api.Services
{
    public interface IFichasService
    {
        Task<IEnumerable<FichaIngresoResponseDto>> GetAllAsync();

        Task<FichaIngresoResponseDto> CreateFichaAsync(FichaIngresoCreateDto dto, int userId);

        Task<FichaResumenDto?> GetResumenAsync(int id);

        Task<bool> AddRepuestoAsync(DetalleRepuestoCreateDto dto);
        Task<bool> AddManoObraAsync(DetalleManoObraCreateDto dto);

        Task<bool> UpdateEstadoAsync(int id, string nuevoEstado);
    }
}