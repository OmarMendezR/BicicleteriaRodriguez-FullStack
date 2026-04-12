using BiciRodriguez.Api.DTOs;

namespace BiciRodriguez.Api.Services
{
    public interface IAuthService
    {
        // Devuelve una cadena (el Token) si las credenciales son correctas
        Task<string?> LoginAsync(string email, string password);
    }
}