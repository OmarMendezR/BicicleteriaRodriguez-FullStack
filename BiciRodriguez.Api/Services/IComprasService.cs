using BiciRodriguez.Api.DTOs;

namespace BiciRodriguez.Api.Interfaces
{
    public interface IComprasService
    {
        Task<bool> RecibirPedidoAsync(int pedidoId);
        Task<IEnumerable<SugerenciaPedidoDto>> GetProductosEnAlertaAsync();
        Task<int> CrearPedidoBorradorAsync(List<SugerenciaPedidoDto> items);
    }
}