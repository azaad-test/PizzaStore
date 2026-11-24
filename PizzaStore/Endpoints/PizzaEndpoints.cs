using PizzaStore.Application.DTOs;
using PizzaStore.Application.Interfaces;
using PizzaStore.Filters;

namespace PizzaStore.Endpoints
{
    public static class PizzaEndpoints
    {
        public static void MapPizzaEndpoints(this WebApplication app)
        {
            app.MapGet("/pizzas", async (IPizzaService service) =>
                Results.Ok(await service.GetAllAsync()));

            var pizzaGroup = app.MapGroup("/pizza");

            pizzaGroup.MapGet("/{id}", async (IPizzaService service, int id) =>
                await service.GetByIdAsync(id) is PizzaResponseDto pizza
                    ? Results.Ok(pizza)
                    : Results.NotFound());

            pizzaGroup.MapPost("/", async (IPizzaService service, CreatePizzaDto dto) =>
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/pizza/{created.Id}", created);
            })
            .AddEndpointFilter<ValidationFilter<CreatePizzaDto>>()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

            pizzaGroup.MapPost("/{pizzaId}/toppings", async (IPizzaService service, int pizzaId, CreateToppingDto dto) =>
            {
                var created = await service.AddToppingAsync(pizzaId, dto);
                return created is not null
                    ? Results.Created($"/pizza/{pizzaId}/toppings/{created.Id}", created)
                    : Results.NotFound($"Pizza with ID {pizzaId} was not found.");
            })
            .AddEndpointFilter<ValidationFilter<CreateToppingDto>>()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

            pizzaGroup.MapPut("/{id}", async (IPizzaService service, UpdatePizzaDto dto, int id) =>
            {
                var updated = await service.UpdateAsync(id, dto);
                return updated ? Results.NoContent() : Results.NotFound();
            })
            .AddEndpointFilter<ValidationFilter<UpdatePizzaDto>>()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

            pizzaGroup.MapDelete("/{id}", async (IPizzaService service, int id) =>
            {
                var deleted = await service.DeleteAsync(id);
                return deleted ? Results.Ok() : Results.NotFound();
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
        }
    }
}