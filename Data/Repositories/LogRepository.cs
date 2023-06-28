using RestaurantOrderingSystem.Data.Repositories.Interface;
using RestaurantOrderingSystem.Models;

namespace RestaurantOrderingSystem.Data.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly DataContext _dataContext;

        public LogRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task SaveLogAsync(Log log)
        {
            await _dataContext.Logs.AddAsync(log);
            await _dataContext.SaveChangesAsync();
        }
        public IEnumerable<Log> GetLogs()
        {
            return _dataContext.Logs.ToList();
        }
    }

}
