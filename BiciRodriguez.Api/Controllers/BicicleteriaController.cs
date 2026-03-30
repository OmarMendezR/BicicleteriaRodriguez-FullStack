using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiciRodriguez.Api.Models; //Referencia de los modelos a usar

namespace BiciRodriguez.Api.Controllers
{
    // Atributos de clase
    [Route("api/[controller]")]
    [ApiController]
    public class BicicleteriaController : ControllerBase
    {
        private readonly BiciContext _context; // Puente con la DB

        // Constructor (Inyeccion de dependencias)
        public BicicleteriaController(BiciContext context)
        {
            _context = context;
        }
        #region METODOS DE LECTURA (GET)

        // Acción para listar todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletas()
        {
            // Consulta Asíncrona
            return await _context.Bicicletas.ToListAsync();
        }

        // GET by ID
        [HttpGet("{id}")] // El {id} se recibira un parametro desde el enpoint
        public async Task<ActionResult<Bicicleta>> GetBicicleta(int id)
        {
            // Busca en la DB por su primary key
            var bicicleta = await _context.Bicicletas.FindAsync(id);

            // de no existir devolvemos un error 404
            if (bicicleta == null)
            {
                return NotFound(new { mensaje = $"La bicicleta con ID {id} no fue encontrada." });
            }

            return bicicleta;
        }
        #endregion
    }
}