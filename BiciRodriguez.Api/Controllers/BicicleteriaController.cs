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

        #endregion
    }
}