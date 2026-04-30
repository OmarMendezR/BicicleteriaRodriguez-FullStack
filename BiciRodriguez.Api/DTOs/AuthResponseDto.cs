namespace BiciRodriguez.Api.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}