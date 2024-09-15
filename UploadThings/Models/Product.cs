using System.ComponentModel.DataAnnotations;

namespace UploadThings.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Type of product is required")]
        public string TypeOfProduct { get; set; }

        public string ProductImagePath { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public Decimal Price { get; set; }
    }
}
