using RestaurantOrderingSystem.Models;
using System.Collections.Generic;

namespace RestaurantOrderingSystem.Data.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<Order?> GetOrderById(int id);
        Task<int> CreateOrder(Order order);
        Task UpdateOrder(Order order);
        Task DeleteOrder(int id);
        Task<bool> SaveChanges();
        Task UpdateOrderStatusAndLog(Order order);
        Task UpdateOrderStatusAsync(int orderId);
    }
}
