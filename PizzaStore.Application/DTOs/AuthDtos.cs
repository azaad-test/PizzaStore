using System.ComponentModel.DataAnnotations;

namespace PizzaStore.Application.DTOs
{
    public record RegisterRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters.")]
        public string Username { get; init; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; init; } = string.Empty;

        public string Role { get; init; } = "Customer";
    }

    public record LoginRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; init; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; init; } = string.Empty;
    }

    public record LoginResponseDto(
        string Username,
        string Role,
        string Token
    );
}