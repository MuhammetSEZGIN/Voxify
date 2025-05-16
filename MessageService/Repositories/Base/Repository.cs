using System;
using MessageService.Data;
using MessageService.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MessageService.Repositories.Base;

public class Repository<T, TId> : IRepository<T, TId> where T : class
{
    protected readonly IMongoCollection<T> _collection;
    protected readonly string _idFieldName = "_id";
    public Repository(IMongoDbContext context, string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            collectionName = typeof(T).Name + "s";
        _collection = context.GetCollection<T>(collectionName);
    }
    public async Task<T> AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(TId id)
    {
        var filter = Builders<T>.Filter.Eq(_idFieldName, id);
        var result = await _collection.DeleteOneAsync(filter);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public Task<T> GetByIdAsync(TId id)
    {
        var filter = Builders<T>.Filter.Eq(_idFieldName, id);
        return _collection.Find(filter).FirstOrDefaultAsync();  
    }

    public async Task<bool> UpdateAsync(TId id,T entity)
    {
        var filter = Builders<T>.Filter.Eq(_idFieldName, id);
        var result = await _collection.ReplaceOneAsync(filter, entity);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

 
}
