using PizzaStore.Application.DTOs;

namespace PizzaStore.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    }
}