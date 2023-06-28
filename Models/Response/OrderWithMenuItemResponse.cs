using System.Text.Json.Serialization;

namespace RestaurantOrderingSystem.Models.Response
{
    public class OrderWithMenuItemsResponse
    {
        public int OrderID { get; set; }
        public List<MenuItemDetails> MenuItemDetail { get; set; }
        public decimal TotalPrice { get; set; }                
    }
    public class MenuItemDetails
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
    }
}
