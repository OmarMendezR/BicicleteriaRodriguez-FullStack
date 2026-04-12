using BiciRodriguez.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BiciRodriguez.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly BiciContext _context;
        private readonly IConfiguration _config;

        public AuthService(BiciContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            // 1. Buscar al usuario por email
            var usuario = await _context.Usuarios.Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email && (u.Activo ?? false));

            if (usuario == null) return null;

            // 2. Verificar la contraseña (El 20% de Pareto: Comparar el Hash)
            // BCrypt.Verify toma la clave plana y la compara con el hash de la DB
            if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
                return null;

            // 3. Si todo está bien, generar el "Brazalete" (JWT)
            return GenerarToken(usuario);
        }

        private string GenerarToken(Usuario usuario)
        {
            // Los "Claims" son los datos que van dentro del brazalete
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre),
                new Claim("id", usuario.UsuarioId.ToString()) // Este es el que usaremos en los controladores
            };

            // La "Key" es la firma secreta para que nadie falsifique el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8), // El brazalete dura 8 horas
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}