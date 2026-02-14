using System;
using MassTransit.Configuration;
using MessageService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;

namespace MessageService.Data;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase Database;
    private readonly IMongoClient Client;
    public IMongoCollection<User> Users { get; }
    public IMongoCollection<Message> Messages { get; }

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        Client = new MongoClient(options.Value.ConnectionString);
        Database = Client.GetDatabase(options.Value.DatabaseName);
        Users = Database.GetCollection<User>(options.Value.UsersCollection);
        Messages = Database.GetCollection<Message>(options.Value.MessagesCollection);
        OnCreating();
    }

    private void OnCreating()
    {
        // ChannelId + CreatedAt için bileşik indeks (sıralama optimizasyonu)
        var compoundIndexDefinition = Builders<Message>
            .IndexKeys.Ascending(m => m.ChannelId)
            .Descending(m => m.CreatedAt);
        var compoundIndexOptions = new CreateIndexOptions { Name = "ChannelId_CreatedAt_Index" };
        var compoundIndexModel = new CreateIndexModel<Message>(
            compoundIndexDefinition,
            compoundIndexOptions
        );
        Messages.Indexes.CreateOne(compoundIndexModel);

        // SenderId için indeks (kullanıcı bazlı sorgular için)
        var senderIndexKeysDefinition = Builders<Message>.IndexKeys.Ascending(m => m.SenderId);
        var senderIndexOptions = new CreateIndexOptions { Name = "SenderId_Index" };
        var senderIndexModel = new CreateIndexModel<Message>(
            senderIndexKeysDefinition,
            senderIndexOptions
        );
        Messages.Indexes.CreateOne(senderIndexModel);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Collection name cannot be null or empty", nameof(name));

        return Database.GetCollection<T>(name);
    }
}
