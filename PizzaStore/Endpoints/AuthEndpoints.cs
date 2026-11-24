using PizzaStore.Application.DTOs;
using PizzaStore.Application.Interfaces;
using PizzaStore.Filters;

namespace PizzaStore.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/register", async (IAuthService authService, RegisterRequestDto dto) =>
            {
                var result = await authService.RegisterAsync(dto);
                return result is not null
                    ? Results.Created($"/users/{result.Username}", result)
                    : Results.Conflict($"Username '{dto.Username}' is already taken.");
            })
            .AddEndpointFilter<ValidationFilter<RegisterRequestDto>>();

            app.MapPost("/login", async (IAuthService authService, LoginRequestDto dto) =>
            {
                var result = await authService.LoginAsync(dto);
                return result is not null
                    ? Results.Ok(result)
                    : Results.Unauthorized();
            })
            .AddEndpointFilter<ValidationFilter<LoginRequestDto>>();
        }
    }
}