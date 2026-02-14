using System;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace MessageService.Interfaces;

public interface IRepository<T, TId> where T : class
{
    Task<T> GetByIdAsync(TId id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<bool> UpdateAsync(TId id,T entity);
    Task<bool> DeleteAsync(TId id);
}
