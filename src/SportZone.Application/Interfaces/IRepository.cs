using System.Linq.Expressions;
using SportZone.Application.DTOs.Common;

namespace SportZone.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();

    // El conteo, filtro de texto, orden y Skip/Take se calculan en Infrastructure (EF Core);
    // Application solo recibe el resultado ya armado.
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(PaginacionQueryDto query);

    // Permite validar unicidad (ej. nombre duplicado) sin exponer EF Core a Application.
    // Expression<Func<T,bool>> es de System.Linq, no de EF Core, así que no rompe Clean Architecture.
    Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado);

    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
