Restaurant Ordering System
This is a simplified version of a restaurant ordering system built with .NET 6 and Entity Framework Core. The system allows managing food menu items and processing customer orders through RESTful APIs.

## Prerequisites
Before running the application, ensure you have the following prerequisites installed on your system:

## Dependencies

This project uses the following NuGet packages:
- Hangfire.AspNetCore v1.8.3
- Hangfire.MemoryStorage v1.8.0
- Hangfire.SqlServer v1.8.3
- Microsoft.EntityFrameworkCore v7.0.8
- Microsoft.EntityFrameworkCore.InMemory v7.0.8
- Swashbuckle.AspNetCore v6.5.0

## Installation and Setup

1. Clone the repository:
   `git clone https://github.com/your-username/restaurant-ordering-system.git`

2. Navigate to the project directory:
   cd restaurant-ordering-system

3. Build the project:
   dotnet build
   
5. Run the project:
   dotnet run

## API Endpoints
The following API endpoints are available:

## Menu API

- GET `/api/menu`: Retrieve all menu items.
- GET `/api/menu/{id}`: Retrieve a specific menu item by ID.
- POST `/api/menu`: Create a new menu item.
- PUT `/api/menu/{id}`: Update a menu item.
- DELETE `/api/menu/{id}`: Delete a menu item.

## Orders API

- GET `/api/orders`: Retrieve all orders.
- GET `/api/orders/{id}`: Retrieve a specific order by ID.
- POST `/api/orders`: Create a new order.
- PUT `/api/orders/{id}`: Update an order.
- DELETE `/api/orders/{id}`: Delete an order.

## Database Configuration
The project is currently configured to use an in-memory database for development purposes. If you want to use a different database, you can modify the database connection string in the appsettings.json file.

## Logging
The project utilizes logging to capture important events and errors. The log files are stored also in-memory database.

## API Documentation
The API documentation is available in Postman JSON format. You can find the file `Restaurant API.postman_collection.json` in the repository.
