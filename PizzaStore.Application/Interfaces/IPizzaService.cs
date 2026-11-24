using PizzaStore.Application.DTOs;

namespace PizzaStore.Application.Interfaces
{
    public interface IPizzaService
    {
        Task<List<PizzaResponseDto>> GetAllAsync();
        Task<PizzaResponseDto?> GetByIdAsync(int id);
        Task<PizzaResponseDto> CreateAsync(CreatePizzaDto dto);
        Task<ToppingResponseDto?> AddToppingAsync(int pizzaId, CreateToppingDto dto);
        Task<bool> UpdateAsync(int id, UpdatePizzaDto dto);
        Task<bool> DeleteAsync(int id);
    }
}