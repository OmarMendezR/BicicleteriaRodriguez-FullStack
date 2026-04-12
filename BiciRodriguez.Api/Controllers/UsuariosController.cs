using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiciRodriguez.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosService _service;

        public UsuariosController(IUsuariosService service)
        {
            _service = service;
        }

        #region LECTURA
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("rol/{rolId}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosPorRol(int rolId)
        {
            return Ok(await _service.GetByRolAsync(rolId));
        }
        #endregion

        #region ESCRITURA
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            try
            {
                var nuevo = await _service.CreateAsync(usuario);
                return CreatedAtAction(nameof(GetUsuarios), new { id = nuevo.UsuarioId }, nuevo);
            }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            var exito = await _service.UpdateAsync(id, usuario);
            if (!exito) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var exito = await _service.DeleteAsync(id);
                if (!exito) return NotFound();
                return Ok(new { mensaje = "Usuario eliminado correctamente" });
            }
            catch (Exception ex) { return BadRequest(new { mensaje = ex.Message }); }
        }
        #endregion
    }
}