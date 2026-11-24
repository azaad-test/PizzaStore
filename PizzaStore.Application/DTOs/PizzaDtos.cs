using System.ComponentModel.DataAnnotations;

namespace PizzaStore.Application.DTOs
{
    public record CreatePizzaDto
    {
        [Required(ErrorMessage = "Pizza name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string Name { get; init; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; init; }
    }

    public record UpdatePizzaDto
    {
        [Required(ErrorMessage = "Pizza name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string Name { get; init; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; init; }
    }

    public record CreateToppingDto
    {
        [Required(ErrorMessage = "Topping name is required.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Topping must be between 2 and 40 characters.")]
        public string Name { get; init; } = string.Empty;
    }

    public record ToppingResponseDto(int Id, string Name);

    public record PizzaResponseDto(
        int Id,
        string Name,
        string? Description,
        List<ToppingResponseDto> Toppings
    );
}