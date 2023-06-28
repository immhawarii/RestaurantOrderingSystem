using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data.Repositories;
using RestaurantOrderingSystem.Controllers;
using RestaurantOrderingSystem.Data.Repositories.Interface;
using Siloam.Service.InvoiceSettlement.Middlewares;
using Hangfire.MemoryStorage;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseInMemoryDatabase("RestaurantDb");
});
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

builder.Services.AddScoped<OrdersController>();
builder.Services.AddScoped<MenuController>();
builder.Services.AddScoped<LogController>();

// Configure Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage());

builder.Services.AddHangfireServer();

// Register the Swagger generator
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Restaurant Ordering API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to read HttpRequest body.
app.UseMiddleware<EnableRequestBodyBufferingMiddleware>();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Ordering API V1");
});

app.UseAuthorization();

app.MapControllers();

// Start Hangfire server
app.UseHangfireDashboard();

app.UseCors(policy => {
    policy.WithOrigins("http://127.0.0.1:5500") // Specify the allowed origin here
          .AllowAnyHeader()
          .AllowAnyMethod();
});

app.Run();
