using BiciRodriguez.Api.Models;

namespace BiciRodriguez.Api.Interfaces
{
    public interface IBalancesService
    {
        Task<bool> GenerarCierreDiarioAsync(int? usuarioId);
        Task<IEnumerable<Balance>> GetHistorialAsync();
        // Agregamos este para que coincida con la implementación que te daré abajo
        Task<bool> GenerarCierreMensualAsync(int anio, int mes, int? usuarioId);
    }
}