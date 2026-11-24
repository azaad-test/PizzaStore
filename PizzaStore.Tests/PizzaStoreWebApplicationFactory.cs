using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PizzaStore.Infrastructure.Persistence;

namespace PizzaStore.Tests
{
    public class PizzaStoreWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = $"PizzaStoreTestDb_{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the production SQL Server DbContext configuration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PizzaDb>));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                // Register an isolated In-Memory database for this test factory
                services.AddDbContext<PizzaDb>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });
            });
        }
    }
}