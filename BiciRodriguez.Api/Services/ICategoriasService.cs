using BiciRodriguez.Api.DTOs;

public interface ICategoriasService
{
    Task<IEnumerable<CategoriaDto>> GetAllAsync();
}