using System.ComponentModel.DataAnnotations;
using UploadThingsGrpcService.Domain.Interfaces;

namespace UploadThingsGrpcService.Domain.Entities
{
    // User model inherit IEntity
    public class User : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is User user)
            {
                return Id == user.Id && Name == user.Name && Email == user.Email;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Email);
        }
    }

}
