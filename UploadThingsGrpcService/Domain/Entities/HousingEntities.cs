using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using UploadThingsGrpcService.Domain.Interfaces;

namespace UploadThingsGrpcService.Domain.Entities
{
    public class HousingLocation : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Photo is required")]
        public string? Photo { get; set; }

        public int AvailableUnits { get; set; }
        public bool Wifi { get; set; }
        public bool Laundry { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is HousingLocation housingLocation)
            {
                return Id == housingLocation.Id &&
                    Name == housingLocation.Name &&
                    City == housingLocation.City &&
                    State == housingLocation.State &&
                    Photo == housingLocation.Photo &&
                    AvailableUnits == housingLocation.AvailableUnits &&
                    Wifi == housingLocation.Wifi &&
                    Laundry == housingLocation.Laundry;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, City, State, Photo);
        }
    }
}
