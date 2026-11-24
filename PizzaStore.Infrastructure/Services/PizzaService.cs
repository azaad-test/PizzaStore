using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.DTOs;
using PizzaStore.Application.Interfaces;
using PizzaStore.Domain.Entities;
using PizzaStore.Infrastructure.Persistence;

namespace PizzaStore.Infrastructure.Services
{
    public class PizzaService : IPizzaService
    {
        private readonly PizzaDb _db;

        public PizzaService(PizzaDb db)
        {
            _db = db;
        }

        public async Task<List<PizzaResponseDto>> GetAllAsync()
        {
            return await _db.Pizzas
                .Include(p => p.Toppings)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        public async Task<PizzaResponseDto?> GetByIdAsync(int id)
        {
            var pizza = await _db.Pizzas
                .Include(p => p.Toppings)
                .FirstOrDefaultAsync(p => p.Id == id);

            return pizza is null ? null : MapToDto(pizza);
        }

        public async Task<PizzaResponseDto> CreateAsync(CreatePizzaDto dto)
        {
            var pizza = new Pizza
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _db.Pizzas.AddAsync(pizza);
            await _db.SaveChangesAsync();

            return MapToDto(pizza);
        }

        public async Task<ToppingResponseDto?> AddToppingAsync(int pizzaId, CreateToppingDto dto)
        {
            var pizza = await _db.Pizzas.FindAsync(pizzaId);
            if (pizza is null) return null;

            var topping = new Topping
            {
                Name = dto.Name,
                PizzaId = pizzaId
            };

            await _db.Toppings.AddAsync(topping);
            await _db.SaveChangesAsync();

            return new ToppingResponseDto(topping.Id, topping.Name);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePizzaDto dto)
        {
            var pizza = await _db.Pizzas.FindAsync(id);
            if (pizza is null) return false;

            pizza.Name = dto.Name;
            pizza.Description = dto.Description;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pizza = await _db.Pizzas.FindAsync(id);
            if (pizza is null) return false;

            _db.Pizzas.Remove(pizza);
            await _db.SaveChangesAsync();

            return true;
        }

        private static PizzaResponseDto MapToDto(Pizza pizza) =>
            new(
                pizza.Id,
                pizza.Name ?? string.Empty,
                pizza.Description,
                pizza.Toppings.Select(t => new ToppingResponseDto(t.Id, t.Name)).ToList()
            );
    }
}