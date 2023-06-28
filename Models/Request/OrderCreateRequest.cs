using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingSystem.Models.Request
{
    public class OrderCreateRequest
    {
        [Required]
        public List<int> MenuItemIds { get; set; }
    }
}
