using BiciRodriguez.Api.Models;
using BiciRodriguez.Api.DTOs;
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

        #region METODOS DE LECTURA
        public async Task<IEnumerable<BicicletaResponseDto>> GetAllAsync()
        {
            return await _context.Bicicletas
                .Select(b => new BicicletaResponseDto
                {
                    BicicletaId = b.BicicletaId,
                    Marca = b.Marca,
                    NumeroMarco = b.NumeroMarco,
                    ClienteId = b.ClienteId,
                    Color = b.Color,
                    TipoBicicleta = b.TipoBicicleta
                }).ToListAsync();
        }

        public async Task<BicicletaResponseDto?> GetByIdAsync(int id)
        {
            var b = await _context.Bicicletas.FindAsync(id);
            if (b == null) return null;

            return new BicicletaResponseDto
            {
                BicicletaId = b.BicicletaId,
                Marca = b.Marca,
                NumeroMarco = b.NumeroMarco,
                ClienteId = b.ClienteId,
                Color = b.Color,
                TipoBicicleta = b.TipoBicicleta
            };
        }

        public async Task<IEnumerable<BicicletaResponseDto>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.Bicicletas
                .Where(b => b.ClienteId == clienteId)
                .Select(b => new BicicletaResponseDto
                {
                    BicicletaId = b.BicicletaId,
                    Marca = b.Marca,
                    NumeroMarco = b.NumeroMarco,
                    ClienteId = b.ClienteId,
                    Color = b.Color,
                    TipoBicicleta = b.TipoBicicleta
                }).ToListAsync();
        }
        #endregion

        #region METODOS DE ESCRITURA
        public async Task<BicicletaResponseDto> CreateAsync(BicicletaResponseDto biciDto, int userId)
        {
            bool existeMarco = await _context.Bicicletas.AnyAsync(b => b.NumeroMarco == biciDto.NumeroMarco);
            if (existeMarco) throw new Exception($"El número de marco '{biciDto.NumeroMarco}' ya está registrado.");

            var nuevaBici = new Bicicleta
            {
                Marca = biciDto.Marca,
                NumeroMarco = biciDto.NumeroMarco,
                ClienteId = (biciDto.ClienteId == 0) ? null : biciDto.ClienteId,
                Color = string.IsNullOrWhiteSpace(biciDto.Color) ? "N/A" : biciDto.Color,
                TipoBicicleta = string.IsNullOrWhiteSpace(biciDto.TipoBicicleta) ? "General" : biciDto.TipoBicicleta,
                Modelo = "N/A",
                FechaRegistro = DateTime.UtcNow,
                UltimaModificacion = DateTime.UtcNow,
                CreadoPorUsuarioId = userId
            };

            _context.Bicicletas.Add(nuevaBici);
            await _context.SaveChangesAsync();

            biciDto.BicicletaId = nuevaBici.BicicletaId;
            return biciDto;
        }

        public async Task<bool> UpdateAsync(int id, BicicletaResponseDto biciDto)
        {
            var bicicletaExistente = await _context.Bicicletas.FindAsync(id);
            if (bicicletaExistente == null) return false;

            // BLINDAJE: Validar Cliente si existe
            if (biciDto.ClienteId.HasValue && biciDto.ClienteId != 0)
            {
                var clienteExiste = await _context.Clientes.AnyAsync(c => c.ClienteId == biciDto.ClienteId);
                if (!clienteExiste) throw new Exception($"El Cliente ID {biciDto.ClienteId} no existe.");
            }

            // BLINDAJE: Validar duplicado de marco
            if (bicicletaExistente.NumeroMarco != biciDto.NumeroMarco)
            {
                bool marcoEnUso = await _context.Bicicletas
                    .AnyAsync(b => b.NumeroMarco == biciDto.NumeroMarco && b.BicicletaId != id);
                if (marcoEnUso) throw new Exception("El nuevo número de marco ya pertenece a otra bicicleta.");
            }

            bicicletaExistente.Marca = biciDto.Marca;
            bicicletaExistente.NumeroMarco = biciDto.NumeroMarco;
            bicicletaExistente.ClienteId = (biciDto.ClienteId == 0) ? null : biciDto.ClienteId;
            bicicletaExistente.Color = string.IsNullOrWhiteSpace(biciDto.Color) ? "N/A" : biciDto.Color;
            bicicletaExistente.TipoBicicleta = string.IsNullOrWhiteSpace(biciDto.TipoBicicleta) ? "General" : biciDto.TipoBicicleta;
            bicicletaExistente.UltimaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bicicleta = await _context.Bicicletas.FindAsync(id);
            if (bicicleta == null) return false;

            try
            {
                _context.Bicicletas.Remove(bicicleta);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("No se puede eliminar la bicicleta porque tiene registros relacionados (fichas, mantenimientos, etc).", ex);
            }
        }
        #endregion
    }
}