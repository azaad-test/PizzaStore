namespace PizzaStore.Domain.Entities
{
    public class Pizza
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Topping> Toppings { get; set; } = new();
    }
}