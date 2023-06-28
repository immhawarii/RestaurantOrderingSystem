using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Data.Repositories;
using RestaurantOrderingSystem.Data.Repositories.Interface;
using RestaurantOrderingSystem.Models;
using RestaurantOrderingSystem.Models.Request;
using System.Text;
using System.Text.Json;

namespace RestaurantOrderingSystem.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly ILogger<MenuController> _logger;
        private readonly ILogRepository _requestLogRepository;

        public MenuController(IMenuRepository menuRepository, ILogger<MenuController> logger, ILogRepository requestLogRepository)
        {
            _menuRepository = menuRepository;
            _logger = logger;
            _requestLogRepository = requestLogRepository;
        }

        // GET: api/menu
        [HttpGet]
        public async Task<ActionResult<BaseResponse<IEnumerable<MenuItem>>>> GetMenuItems()
        {
            try
            {
                var data = await _menuRepository.GetAllMenuItems();
                var result = new BaseResponse<IEnumerable<MenuItem>>(200, "Success", "Menu items retrieved successfully", data);

                _logger.LogInformation("Menu items retrieved successfully.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu items.");
                var errRes = new BaseResponse<IEnumerable<MenuItem>>(500, "Error", "Internal server error", null);
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));
                return errRes;
            }
        }

        // GET: api/menu/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<MenuItem>>> GetMenuItem(int id)
        {
            try
            {
                var menuItem = await _menuRepository.GetMenuItemById(id);

                if (menuItem == null)
                {
                    _logger.LogWarning($"Menu item with ID {id} not found.");
                    var errRes = new BaseResponse<MenuItem>(404, "Not Found", "Menu item not found", null);
                    await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));

                    return NotFound(errRes);
                }

                var result = new BaseResponse<MenuItem>(200, "Success", "Menu item retrieved successfully", menuItem);

                _logger.LogInformation($"Menu item with ID {id} retrieved successfully.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(result));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu item.");
                var errRes = new BaseResponse<MenuItem>(500, "Error", "Internal server error", null);
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));
                return errRes;
            }
        }

        // POST: api/menu
        [HttpPost]
        public async Task<ActionResult<BaseResponse<MenuItem>>> PostMenuItem(MenuItemCreateRequest request)
        {
            try
            {
                var menuItem = new MenuItem
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price
                };

                await _menuRepository.CreateMenuItem(menuItem);
                var result = new BaseResponse<MenuItem>(201, "Success", "Menu item created successfully", menuItem);

                _logger.LogInformation($"Menu item with ID {menuItem.ID} created successfully.");                
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating menu item.");
                var errRes = new BaseResponse<MenuItem>(500, "Error", "Internal server error", null);
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));
                return errRes;
            }
        }

        // PUT: api/menu/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<MenuItem>>> PutMenuItem(int id, MenuItemCreateRequest request)
        {
            try
            {
                var menuItem = new MenuItem
                {
                    ID = id,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price
                };

                var existingItem = await _menuRepository.GetMenuItemById(id);
                if (existingItem == null)
                {
                    _logger.LogWarning($"Menu item with ID {id} not found.");
                    var errRes= new BaseResponse<MenuItem>(404, "Not Found", "Menu item not found", null);
                    await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));

                    return NotFound(errRes);
                }

                await _menuRepository.UpdateMenuItem(menuItem);
                var successRes = new BaseResponse<MenuItem>(200, "Success", "Menu item updated successfully", menuItem);

                _logger.LogInformation($"Menu item with ID {id} updated successfully.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(successRes));
                return successRes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating menu item.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, "");
                return new BaseResponse<MenuItem>(500, "Error", "Internal server error", null);
            }
        }

        // DELETE: api/menu/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<MenuItem>>> DeleteMenuItem(int id)
        {
            try
            {
                var existingItem = await _menuRepository.GetMenuItemById(id);
                if (existingItem == null)
                {
                    _logger.LogWarning($"Menu item with ID {id} not found.");
                    var errRes = new BaseResponse<MenuItem>(404, "Not Found", "Menu item not found", null);
                    await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));

                    return NotFound(errRes);
                }

                await _menuRepository.DeleteMenuItem(id);
                var result = new BaseResponse<MenuItem>(200, "Success", "Menu item deleted successfully", existingItem);
                
                _logger.LogInformation($"Menu item with ID {id} deleted successfully.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting menu item.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, "");
                return new BaseResponse<MenuItem>(500, "Error", "Internal server error", null);
            }
        }
        private async Task LogRequest(string path, string method, HttpRequest request, int statusCode, string resultBody)
        {
            try
            {
                string requestBody = "";

                request.EnableBuffering();
                request.Body.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new(request.Body, Encoding.UTF8))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                Log log = new()
                {
                    RequestPath = path,
                    HttpMethod = method,
                    RequestTime = DateTime.Now,
                    RequestBody = requestBody,
                    StatusCode = statusCode,
                    ResponseTime = DateTime.Now,
                    ResultBody = resultBody
                };
                await _requestLogRepository.SaveLogAsync(log);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}
