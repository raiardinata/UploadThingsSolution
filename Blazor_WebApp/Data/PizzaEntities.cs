namespace Blazor_WebApp.Data
{
    public class PizzaEntities
    {
        public int PizzaID { get; set; }
        public string? PizzaName { get; set; }
        public string? PizzaDescription { get; set; }
        public decimal PizzaPrice { get; set; } = 0;
        public bool Vegetarian { get; set; }
        public bool Vegan { get; set; }
    }
}
