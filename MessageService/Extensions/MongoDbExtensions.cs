using System;
using MessageService.Data;
using MessageService.Interfaces.Repositories.IUserRepository;
using MessageService.Models;
using MessageService.Repositories;

namespace MessageService.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDbService
    (
        this IServiceCollection services,
        IConfiguration configuration)
    {

        var mongoSettings = configuration.GetSection("MongoDbConnection").Get<MongoDbSettings>();
        if (mongoSettings == null || string.IsNullOrEmpty(mongoSettings.ConnectionString))
        {
            var message = mongoSettings == null
                ? "MongoDB configuration section not found"
                : "MongoDB ConnectionString is missing";
            throw new InvalidOperationException(message);
        }


        services.Configure<MongoDbSettings>(
            configuration.GetSection("MongoDbConnection"));

        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        Console.WriteLine($"MongoDB connection string from config: {mongoSettings?.ConnectionString ?? "NULL"}");
        services.AddScoped<IUserRepository>(sp =>
            new UserRepository(sp.GetRequiredService<IMongoDbContext>(),
            configuration.GetSection("UserCollection").Value
            ));

        services.AddScoped<IMessageRepository>(sp =>
            new MessageRepository(sp.GetRequiredService<IMongoDbContext>(),
            configuration.GetSection("MessageCollection").Value
            ));
        return services;
    }
}
