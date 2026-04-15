using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;
using BiciRodriguez.Api.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Services
{
    public class BicicletasService : IBicicletasService
    {
        private readonly BiciContext _context;

        public BicicletasService(BiciContext context)
        {
            _context = context;
        }

        #region READ METHODS
        public async Task<IEnumerable<BicicletaResponseDto>> GetAllAsync()
        {
            var bicicletas = await _context.Bicicletas.ToListAsync();
            return bicicletas.Select(b => b.ToDto());
        }

        public async Task<BicicletaResponseDto?> GetByIdAsync(int id)
        {
            var b = await _context.Bicicletas.FindAsync(id);
            return b?.ToDto();
        }

        public async Task<IEnumerable<BicicletaResponseDto>> GetByClienteIdAsync(int clienteId)
        {
            var bicicletas = await _context.Bicicletas
                .Where(b => b.ClienteId == clienteId)
                .ToListAsync();

            return bicicletas.Select(b => b.ToDto());
        }
        #endregion

        #region WRITE METHODS
        public async Task<BicicletaResponseDto> CreateAsync(BicicletaResponseDto biciDto, int userId)
        {
            // Validar unicidad del marco
            if (await _context.Bicicletas.AnyAsync(b => b.NumeroMarco == biciDto.NumeroMarco))
                throw new Exception($"El número de marco '{biciDto.NumeroMarco}' ya existe.");

            var nuevaBici = biciDto.ToEntity(userId);

            _context.Bicicletas.Add(nuevaBici);
            await _context.SaveChangesAsync();

            return nuevaBici.ToDto();
        }

        public async Task<bool> UpdateAsync(int id, BicicletaResponseDto biciDto)
        {
            var existente = await _context.Bicicletas.FindAsync(id);
            if (existente == null) return false;

            // Validar cliente si se proporciona uno nuevo
            if (biciDto.ClienteId.HasValue && biciDto.ClienteId != 0)
            {
                if (!await _context.Clientes.AnyAsync(c => c.ClienteId == biciDto.ClienteId))
                    throw new Exception($"El Cliente ID {biciDto.ClienteId} no existe.");
            }

            // Validar que el nuevo marco no lo tenga otra bicicleta
            if (existente.NumeroMarco != biciDto.NumeroMarco)
            {
                if (await _context.Bicicletas.AnyAsync(b => b.NumeroMarco == biciDto.NumeroMarco && b.BicicletaId != id))
                    throw new Exception("El número de marco ya pertenece a otra unidad.");
            }

            // Actualización de campos
            existente.Marca = biciDto.Marca;
            existente.NumeroMarco = biciDto.NumeroMarco;
            existente.ClienteId = (biciDto.ClienteId == 0) ? null : biciDto.ClienteId;
            existente.Color = string.IsNullOrWhiteSpace(biciDto.Color) ? "N/A" : biciDto.Color;
            existente.TipoBicicleta = string.IsNullOrWhiteSpace(biciDto.TipoBicicleta) ? "General" : biciDto.TipoBicicleta;
            existente.UltimaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var b = await _context.Bicicletas.FindAsync(id);
            if (b == null) return false;

            try
            {
                _context.Bicicletas.Remove(b);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: El registro tiene dependencias activas (fichas o mantenimientos).", ex);
            }
        }
        #endregion
    }
}