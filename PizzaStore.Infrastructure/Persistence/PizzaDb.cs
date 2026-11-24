using Microsoft.EntityFrameworkCore;
using PizzaStore.Domain.Entities;

namespace PizzaStore.Infrastructure.Persistence
{
    public class PizzaDb : DbContext
    {
        public PizzaDb(DbContextOptions<PizzaDb> options) : base(options) { }
        public DbSet<Pizza> Pizzas { get; set; } = null!;
        public DbSet<Topping> Toppings { get; set; } = null!;
        public DbSet<ApplicationUser> Users { get; set; } = null!;
    }
}