using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Todos deben estar logueados
public class BicicletasController : ControllerBase
{
    private readonly IBicicletasService _service;

    public BicicletasController(IBicicletasService service)
    {
        _service = service;
    }

    #region READ
    [HttpGet]
    [Authorize(Roles = "Administrador,Mecanico")] // Solo personal del taller ve todo
    public async Task<ActionResult<IEnumerable<BicicletaResponseDto>>> GetBicicletas()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BicicletaResponseDto>> GetBicicleta(int id)
    {
        var b = await _service.GetByIdAsync(id);
        return b == null ? NotFound(new { mensaje = "No encontrada" }) : Ok(b);
    }
    #endregion

    #region WRITE
    [HttpPost]
    [Authorize(Roles = "Administrador,Mecanico")]
    public async Task<ActionResult<BicicletaResponseDto>> PostBicicleta(BicicletaResponseDto biciDto)
    {
        // Uso de extension method o Claim directo de forma segura
        var userIdClaim = User.FindFirst("id")?.Value;
        if (!int.TryParse(userIdClaim, out int currentUserId))
            return Unauthorized(new { mensaje = "Token inválido" });

        try
        {
            var resultado = await _service.CreateAsync(biciDto, currentUserId);
            return CreatedAtAction(nameof(GetBicicleta), new { id = resultado.BicicletaId }, resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
    #endregion

    #region UPDATE
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Mecanico")] // Ambos pueden editar info técnica
    public async Task<IActionResult> PutBicicleta(int id, BicicletaResponseDto biciDto)
    {
        if (id != biciDto.BicicletaId)
        {
            return BadRequest(new { mensaje = "El ID de la URL no coincide con el del cuerpo." });
        }

        try
        {
            var actualizado = await _service.UpdateAsync(id, biciDto);

            if (!actualizado)
                return NotFound(new { mensaje = $"La bicicleta con ID {id} no existe." });

            return NoContent(); // 204: Éxito pero no devuelve contenido (estándar REST)
        }
        catch (Exception ex)
        {
            // Capturamos errores de lógica del servicio (ej: marco duplicado)
            return BadRequest(new { mensaje = ex.Message });
        }
    }
    #endregion

    #region DELETE
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")] // ¡SOLO el Admin borra!
    public async Task<IActionResult> DeleteBicicleta(int id)
    {
        try
        {
            var eliminado = await _service.DeleteAsync(id);
            return eliminado ? Ok(new { mensaje = "Eliminado" }) : NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = ex.Message });
        }
    }
    #endregion
}