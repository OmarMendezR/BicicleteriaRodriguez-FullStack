using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (token == null)
            {
                // Por seguridad, no decimos si falló el correo o la clave, solo "No autorizado"
                return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario inactivo." });
            }

            return Ok(new { token });
        }
    }
}