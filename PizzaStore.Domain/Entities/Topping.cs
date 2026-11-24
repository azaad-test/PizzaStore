namespace PizzaStore.Domain.Entities
{
    public class Topping
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PizzaId { get; set; }
        public Pizza? Pizza { get; set; }
    }
}