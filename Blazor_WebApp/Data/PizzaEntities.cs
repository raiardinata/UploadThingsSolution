namespace Blazor_WebApp.Data
{
    // Outer response class
    public class PizzaSpecialResponse
    {
        public List<PizzaEntities> PizzaSpecialData { get; set; } = new List<PizzaEntities>();
    }

    public class PizzaEntities
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal BasePrice { get; set; } = 0;
        public string? ImageUrl { get; set; }
        public string GetFormattedBasePrice() => BasePrice.ToString("0.00");
    }
}
