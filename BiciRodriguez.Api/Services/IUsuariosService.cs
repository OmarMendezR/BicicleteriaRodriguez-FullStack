using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Models;

namespace BiciRodriguez.Api.Services
{
    public interface IUsuariosService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetByRolAsync(int rolId); // Para listar mecánicos
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<bool> UpdateAsync(int id, Usuario usuario);
        Task<bool> DeleteAsync(int id);
    }
}