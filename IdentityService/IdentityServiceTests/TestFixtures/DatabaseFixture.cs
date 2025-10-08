using IdentityService.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
public class DatabaseFixture : IDisposable
{
    public IdentityDbContext Context { get; private set; }
    public ServiceProvider ServiceProvider { get; private set; }

    public DatabaseFixture()
    {
        var services = new ServiceCollection();
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
        ServiceProvider = services.BuildServiceProvider();
        Context = ServiceProvider.GetRequiredService<IdentityDbContext>();
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
        ServiceProvider.Dispose();
    }
}