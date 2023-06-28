using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingSystem.Models.Request
{
    public class OrderUpdateRequest
    {

        [Required]
        public List<int> MenuItemIds { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
    }
}
