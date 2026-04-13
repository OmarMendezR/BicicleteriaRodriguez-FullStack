namespace BiciRodriguez.Api.DTOs
{
    public class RegisterDto
    {
        public string NombreCompleto { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RolID { get; set; } // Por ahora mandaremos 1 o 2 manualmente
    }
}