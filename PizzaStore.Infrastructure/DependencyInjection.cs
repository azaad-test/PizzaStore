using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PizzaStore.Application.Interfaces;
using PizzaStore.Infrastructure.Persistence;
using PizzaStore.Infrastructure.Services;

namespace PizzaStore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Pizzas")
                ?? "Server=(localdb)\\mssqllocaldb;Database=PizzaStore;Trusted_Connection=True;TrustServerCertificate=True;";

            services.AddDbContext<PizzaDb>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IPizzaService, PizzaService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}