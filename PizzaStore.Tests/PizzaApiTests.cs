using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PizzaStore.Application.DTOs;

namespace PizzaStore.Tests
{
    public class PizzaApiTests : IClassFixture<PizzaStoreWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PizzaApiTests(PizzaStoreWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetPizzas_WithoutToken_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/pizzas");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreatePizza_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            var newPizza = new CreatePizzaDto
            {
                Name = "Margherita",
                Description = "Classic tomato and mozzarella"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/pizza", newPizza);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreatePizza_WithCustomerRole_ReturnsForbidden()
        {
            // Arrange: Register a regular Customer and attach their JWT
            var token = await RegisterAndGetTokenAsync("customer_user", "Password123", "Customer");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newPizza = new CreatePizzaDto
            {
                Name = "Pepperoni",
                Description = "Spicy pepperoni"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/pizza", newPizza);

            // Assert: Authenticated, but lacks the "Admin" role
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreatePizza_WithInvalidName_ReturnsBadRequest()
        {
            // Arrange: Register an Admin, but send a Name shorter than 3 characters
            var token = await RegisterAndGetTokenAsync("admin_validation", "Password123", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var invalidPizza = new CreatePizzaDto
            {
                Name = "Hi",
                Description = "Too short to pass ValidationFilter"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/pizza", invalidPizza);

            // Assert: ValidationFilter blocks the request before hitting PizzaService
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreatePizzaAndTopping_WithAdminRole_PersistsAndReturnsData()
        {
            // Arrange: Register an Admin and attach JWT
            var token = await RegisterAndGetTokenAsync("admin_boss", "Password123", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newPizza = new CreatePizzaDto
            {
                Name = "BBQ Chicken",
                Description = "Smoky BBQ sauce, grilled chicken, red onions"
            };

            // Act 1: Create the Pizza
            var createResponse = await _client.PostAsJsonAsync("/pizza", newPizza);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdPizza = await createResponse.Content.ReadFromJsonAsync<PizzaResponseDto>();
            Assert.NotNull(createdPizza);
            Assert.Equal("BBQ Chicken", createdPizza.Name);

            // Act 2: Add a Topping to that Pizza
            var toppingDto = new CreateToppingDto { Name = "Extra Cheddar" };
            var toppingResponse = await _client.PostAsJsonAsync($"/pizza/{createdPizza.Id}/toppings", toppingDto);
            Assert.Equal(HttpStatusCode.Created, toppingResponse.StatusCode);

            // Act 3: Fetch the Pizza by ID and verify the relational join
            var fetchedPizza = await _client.GetFromJsonAsync<PizzaResponseDto>($"/pizza/{createdPizza.Id}");
            Assert.NotNull(fetchedPizza);
            Assert.Single(fetchedPizza.Toppings);
            Assert.Equal("Extra Cheddar", fetchedPizza.Toppings[0].Name);
        }

        private async Task<string> RegisterAndGetTokenAsync(string username, string password, string role)
        {
            var registerDto = new RegisterRequestDto
            {
                Username = $"{username}_{Guid.NewGuid():N}"[..25],
                Password = password,
                Role = role
            };

            var response = await _client.PostAsJsonAsync("/register", registerDto);
            response.EnsureSuccessStatusCode();

            var authResult = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            return authResult!.Token;
        }
    }
}