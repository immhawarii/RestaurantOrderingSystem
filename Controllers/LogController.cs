using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Data.Repositories.Interface;
using RestaurantOrderingSystem.Models;

namespace RestaurantOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogController : ControllerBase
    {
        private readonly ILogRepository _requestLogRepository;

        public LogController(ILogRepository requestLogRepository)
        {
            _requestLogRepository = requestLogRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Log>> GetAllLogs()
        {
            try
            {
                var logs = _requestLogRepository.GetLogs();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                // Log and handle any exceptions
                // Return appropriate error response
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
