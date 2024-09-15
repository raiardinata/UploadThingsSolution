using System.ComponentModel.DataAnnotations;

namespace UploadThings.Models
{
    public class Transaction
    {
        public int id { get; set; }

        [Required(ErrorMessage = "username is required")]
        public string username { get; set; }

        [Required(ErrorMessage = "productname is required")]
        public string productname { get; set; }

        [Required(ErrorMessage = "quantity is required")]
        public int quantity { get; set; }

        [Required(ErrorMessage = "totalprice is required")]
        public decimal totalprice { get; set; }

        [Required(ErrorMessage = "unitprice is required")]
        public decimal unitprice { get; set; }

        [Required(ErrorMessage = "date is required")]
        public DateTime date { get; set; }
    }
}
