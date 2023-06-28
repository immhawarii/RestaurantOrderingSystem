using RestaurantOrderingSystem.Models;
using RestaurantOrderingSystem.Models.Response;
using System.Collections.Generic;

namespace RestaurantOrderingSystem.Data.Repositories
{
    public interface IMenuRepository
    {
        Task<IEnumerable<MenuItem>> GetAllMenuItems();
        Task<MenuItem?> GetMenuItemById(int id);
        Task CreateMenuItem(MenuItem menuItem);
        Task UpdateMenuItem(MenuItem menuItem);
        Task DeleteMenuItem(int id);
        Task<bool> SaveChanges();
        Task<decimal> CalculateTotalPrice(List<int> menuItemIds);
        Task<List<MenuItemDetails>> GetMenuItemNamesByIds(List<int> menuItemIds);
    }
}
