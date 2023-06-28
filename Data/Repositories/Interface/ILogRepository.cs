using RestaurantOrderingSystem.Models;

namespace RestaurantOrderingSystem.Data.Repositories.Interface
{
    public interface ILogRepository
    {
        Task SaveLogAsync(Log log);
        IEnumerable<Log> GetLogs();
    }
}
