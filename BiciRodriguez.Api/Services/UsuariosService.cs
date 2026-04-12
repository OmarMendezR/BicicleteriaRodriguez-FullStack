using BiciRodriguez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly BiciContext _context;

        public UsuariosService(BiciContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.Include(u => u.Rol).ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetByRolAsync(int rolId)
        {
            return await _context.Usuarios
                .Where(u => u.RolId == rolId && (u.Activo ?? false))
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email);
            if (existe) throw new Exception("El correo electrónico ya está registrado.");

            // Eliminamos la línea de FechaRegistro porque no existe en tu modelo Usuario.cs
            usuario.Activo = usuario.Activo ?? true;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> UpdateAsync(int id, Usuario u)
        {
            var existente = await _context.Usuarios.FindAsync(id);
            if (existente == null) return false;

            existente.NombreCompleto = u.NombreCompleto;
            existente.Email = u.Email;
            existente.RolId = u.RolId;
            existente.Activo = u.Activo;
            // Eliminamos la línea de UltimaModificacion porque no existe en tu modelo Usuario.cs

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            var tieneFichas = await _context.FichasIngresos.AnyAsync(f => f.MecanicoId == id || f.CreadoPorUsuarioId == id);
            if (tieneFichas) throw new Exception("No se puede eliminar: El usuario tiene historial de trabajo. Desactívelo.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}