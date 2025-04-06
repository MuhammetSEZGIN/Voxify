using System;

namespace ClanService.Interfaces.Repositories;

public interface IRepository<T, Tid> where T : class
{
    Task<T> GetByIdAsync(Tid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
