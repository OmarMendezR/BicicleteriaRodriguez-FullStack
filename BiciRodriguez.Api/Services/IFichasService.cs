using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;

namespace BiciRodriguez.Api.Services
{
    public interface IFichasService
    {
        Task<IEnumerable<FichaIngresoResponseDto>> GetAllAsync();
        Task<object?> GetResumenAsync(int id);
        Task<FichasIngreso> CreateFichaAsync(FichaIngresoCreateDto dto, int userId);
        Task<object> AddRepuestoAsync(DetalleRepuestoCreateDto dto);
        Task<object> AddManoObraAsync(DetalleManoObraCreateDto dto);
        Task<bool> UpdateEstadoAsync(int id, string nuevoEstado);
    }
}