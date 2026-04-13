using BiciRodriguez.Api.DTOs;
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
            var usuario = await _context.Usuarios.Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email && (u.Activo ?? false));

            if (usuario == null) return null;

            bool esValida = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
            if (!esValida) return null;

            return GenerarToken(usuario);
        }

        private string GenerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", usuario.UsuarioId.ToString()),
                    new Claim("role", usuario.Rol.Nombre)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == registerDto.Email))
                return false;

            var usuario = new Usuario
            {
                NombreCompleto = registerDto.NombreCompleto,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                RolId = registerDto.RolID,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}