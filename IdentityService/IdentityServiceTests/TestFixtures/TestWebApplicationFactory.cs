using IdentityService.Data;
using IdentityServiceTests.Mocks;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    public MockPublishEndpoint MockPublishEndpoint { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove real database
            var dbContextDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<IdentityDbContext>)
            );
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Remove MassTransit/RabbitMQ
            var massTransitDescriptors = services
                .Where(d => d.ServiceType.Namespace?.Contains("MassTransit") == true)
                .ToList();

            foreach (var descriptor in massTransitDescriptors)
            {
                services.Remove(descriptor);
            }

            // Remove IPublishEndpoint specifically
            var publishEndpointDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IPublishEndpoint)
            );
            if (publishEndpointDescriptor != null)
            {
                services.Remove(publishEndpointDescriptor);
            }

            // Add InMemory database
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            });

            // Add Mock RabbitMQ Publisher
            MockPublishEndpoint = new MockPublishEndpoint();
            services.AddSingleton<IPublishEndpoint>(MockPublishEndpoint);

            // Build and seed database
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                db.Database.EnsureCreated();
            }
        });

        builder.UseEnvironment("Testing");
    }
}
