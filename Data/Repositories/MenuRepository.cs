using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Models;
using RestaurantOrderingSystem.Models.Response;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RestaurantOrderingSystem.Data.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly DataContext _context;

        public MenuRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetAllMenuItems()
        {
            return await _context.MenuItems.ToListAsync();
        }

        public async Task<MenuItem?> GetMenuItemById(int id)
        {
            return await _context.MenuItems.FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task CreateMenuItem(MenuItem menuItem)
        {
            await _context.MenuItems.AddAsync(menuItem);
            await SaveChanges();
        }

        public async Task UpdateMenuItem(MenuItem menuItem)
        {
            var existingMenuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.ID == menuItem.ID);

            if (existingMenuItem != null)
            {
                existingMenuItem.Name = menuItem.Name;
                existingMenuItem.Description = menuItem.Description;
                existingMenuItem.Price = menuItem.Price;

                await SaveChanges();
            }
        }


        public async Task DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(p => p.ID == id);
            if (menuItem != null)
            {
                _context.MenuItems.Remove(menuItem);
                await SaveChanges();
            }
        }
        private async Task<IEnumerable<MenuItem>> GetMenuItemsByIds(List<int> menuItemIds)
        {
            var menuItems = await _context.MenuItems
                .Where(m => menuItemIds.Contains(m.ID))
                .ToListAsync();

            var duplicateMenuItems = new List<MenuItem>();
            foreach (var menuItemId in menuItemIds)
            {
                var menuItem = menuItems.FirstOrDefault(m => m.ID == menuItemId);
                if (menuItem != null)
                {
                    duplicateMenuItems.Add(menuItem);
                }
            }

            return duplicateMenuItems;
        }
        public async Task<List<MenuItemDetails>> GetMenuItemNamesByIds(List<int> menuItemIds)
        {
            return await _context.MenuItems
                .Where(m => menuItemIds.Contains(m.ID))
                .Select(m => new MenuItemDetails() { Name = m.Name, MenuId = m.ID})
                .ToListAsync();
        }

        public async Task<decimal> CalculateTotalPrice(List<int> menuItemIds)
        {
            var menuItems = await GetMenuItemsByIds(menuItemIds);
            decimal totalPrice = menuItems.Sum(m => m.Price);
            return totalPrice;
        }


        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
