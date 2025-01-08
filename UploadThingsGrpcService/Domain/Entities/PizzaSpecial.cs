using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Domain.Interfaces;

namespace UploadThingsGrpcService.Domain.Entities
{
    /// Represents a pre-configured template for a pizza a user can order
    public class PizzaSpecial : IEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        [Precision(18, 4)]
        public decimal BasePrice { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string GetFormattedBasePrice() => BasePrice.ToString("0.00");

        public override bool Equals(object? obj)
        {
            return obj is PizzaSpecial pizzaSpecial && Id == pizzaSpecial.Id && Name == pizzaSpecial.Name && BasePrice == pizzaSpecial.BasePrice && Description == pizzaSpecial.Description && ImageUrl == pizzaSpecial.ImageUrl;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, BasePrice, Description, ImageUrl);
        }

    }
}
