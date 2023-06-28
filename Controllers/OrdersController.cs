using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Data.Repositories;
using RestaurantOrderingSystem.Data.Repositories.Interface;
using RestaurantOrderingSystem.Models;
using System.Text.Json;
using System.Text;
using RestaurantOrderingSystem.Models.Request;
using RestaurantOrderingSystem.Models.Response;
using Hangfire;

namespace RestaurantOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrdersController> _logger;
        private readonly ILogRepository _requestLogRepository;
        private readonly IMenuRepository _menuRepository;

        public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger, ILogRepository requestLogRepository, IMenuRepository menuRepository)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _requestLogRepository = requestLogRepository;
            _menuRepository = menuRepository;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<BaseResponse<IEnumerable<Order>>>> GetOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrders();
                var result = new BaseResponse<IEnumerable<Order>>(200, "Success", "Orders retrieved successfully", orders);

                _logger.LogInformation("Orders retrieved successfully.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders.");
                var errRes = new BaseResponse<IEnumerable<Order>>(500, "Error", "Internal server error", null);
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));
                return errRes;
            }
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<Order>>> GetOrder(int id)
        {
            try
            {
                var order = await _orderRepository.GetOrderById(id);

                if (order == null)
                {
                    _logger.LogWarning($"Order with ID {id} not found.");
                    var errRes = new BaseResponse<Order>(404, "Not Found", "Order not found", null);
                    await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));

                    return NotFound(errRes);
                }

                var result = new BaseResponse<Order>(200, "Success", "Order retrieved successfully", order);

                _logger.LogInformation($"Order with ID {id} retrieved successfully.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(result));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order.");
                var errRes = new BaseResponse<Order>(500, "Error", "Internal server error", null);
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));
                return errRes;
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<BaseResponse<OrderWithMenuItemsResponse>>> CreateOrder(OrderCreateRequest request)
        {
            try
            {
                // Validate the menu items in the order exist in the menu before accepting the order
                var invalidMenuItems = request.MenuItemIds.Except((await _menuRepository.GetAllMenuItems()).Select(m => m.ID));
                if (invalidMenuItems.Any())
                {
                    return BadRequest("Invalid menu items in the order");
                }

                decimal totalPrice = await _menuRepository.CalculateTotalPrice(request.MenuItemIds);
                var menutItemDetails = await _menuRepository.GetMenuItemNamesByIds(request.MenuItemIds);

                var orderItem = new Order
                {
                    MenuItemIds = request.MenuItemIds,
                    TotalPrice = totalPrice,
                    Status = "Placed"
                };

                int orderId = await _orderRepository.CreateOrder(orderItem);

                // Enqueue a Hangfire background job to update the order status
                var jobId = BackgroundJob.Schedule(() => _orderRepository.UpdateOrderStatusAsync(orderId), TimeSpan.FromSeconds(5));

                //// Start the asynchronous process to update the order status after a delay
                //await _orderRepository.UpdateOrderStatusAsync(orderId);

                var response = new OrderWithMenuItemsResponse
                {
                    OrderID = orderId,
                    MenuItemDetail = menutItemDetails,
                    TotalPrice = totalPrice
                };

                var result = new BaseResponse<OrderWithMenuItemsResponse>(201, "Success", "Order created successfully", response);

                _logger.LogInformation($"Order with ID {orderId} created successfully.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order.");
                var errRes = new BaseResponse<OrderWithMenuItemsResponse>(500, "Error", "Internal server error", null);
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));
                return errRes;
            }
        }


        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<Order>>> UpdateOrder(int id, Order order)
        {
            try
            {
                if (id != order.OrderID)
                {
                    return BadRequest();
                }

                await _orderRepository.UpdateOrder(order);

                var result = new BaseResponse<Order>(200, "Success", "Order updated successfully", order);
                _logger.LogInformation($"Order with ID {id} updated successfully.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(result));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order.");
                var errRes = new BaseResponse<Order>(500, "Error", "Internal server error", null);
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));
                return errRes;
            }
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<Order>>> DeleteOrder(int id)
        {
            try
            {
                await _orderRepository.DeleteOrder(id);

                var result = new BaseResponse<Order>(200, "Success", "Order deleted successfully", null);
                _logger.LogInformation($"Order with ID {id} deleted successfully.");
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(result));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order.");
                var errRes = new BaseResponse<Order>(500, "Error", "Internal server error", null);
                await LogRequest(Request.Path, Request.Method, HttpContext.Request, Response.StatusCode, JsonSerializer.Serialize(errRes));
                return errRes;
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
