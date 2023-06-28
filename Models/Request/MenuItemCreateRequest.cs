namespace RestaurantOrderingSystem.Models.Request
{
    public class MenuItemCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
