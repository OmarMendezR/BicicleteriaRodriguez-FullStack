using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Services;
using BiciRodriguez.Api.Models; // 1. IMPORTANTE: Agregar este using para BiciContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly BiciContext _context; // 2. Declarar el campo privado

        // 3. Inyectar BiciContext en el constructor
        public AuthController(IAuthService authService, BiciContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (token == null)
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario inactivo." });
            }

            // 4. Ahora _context ya existe. Usamos Include para traer el nombre del Rol.
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (usuario == null) return Unauthorized();

            return Ok(new AuthResponseDto
            {
                Token = token,
                Role = usuario.Rol?.Nombre ?? "Sin Rol", // Extraemos el nombre del rol
                Nombre = usuario.NombreCompleto ?? "Usuario" // Usamos la propiedad de tu modelo
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var resultado = await _authService.RegisterAsync(registerDto);
            if (!resultado)
                return BadRequest(new { mensaje = "El correo ya existe o hubo un error." });

            return Ok(new { mensaje = "Usuario registrado con éxito." });
        }
    }
}