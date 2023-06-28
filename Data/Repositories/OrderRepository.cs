using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantOrderingSystem.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;

        public OrderRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(p => p.OrderID == id);
        }

        public async Task<int> CreateOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
            await SaveChanges();
            return order.OrderID;
        }

        public async Task UpdateOrder(Order order)
        {
            var existingOrder = _context.Orders.FirstOrDefault(o => o.OrderID == order.OrderID);

            if (existingOrder != null)
            {
                existingOrder.MenuItemIds = order.MenuItemIds;
                existingOrder.TotalPrice = order.TotalPrice;
                existingOrder.Status = order.Status;

                await SaveChanges();
            }
        }

        public async Task DeleteOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(p => p.OrderID == id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await SaveChanges();
            }
        }

        public async Task UpdateOrderStatusAndLog(Order order)
        {
            // Update the order status to "In Progress"
            order.Status = "In Progress";
            _context.SaveChanges();

            // Delay the execution for a few seconds
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Log the status change to a text file
            string logMessage = $"Order {order.OrderID} status updated to In Progress";
            await LogStatusChangeToFile(logMessage);
        }

        private static async Task LogStatusChangeToFile(string logMessage)
        {
            string filePath = "order_log.txt";
            using StreamWriter writer = new(filePath, true);
            await writer.WriteLineAsync(logMessage);
        }

        public async Task UpdateOrderStatusAsync(int orderId)
        {
            var order = await GetOrderById(orderId);
            if (order != null && order.Status == "Placed")
            {
                order.Status = "In Progress";
                await UpdateOrder(order);
                await SaveChanges();
            }
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
