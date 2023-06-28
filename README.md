Restaurant Ordering System
This is a simplified version of a restaurant ordering system built with .NET 6 and Entity Framework Core. The system allows managing food menu items and processing customer orders through RESTful APIs.

Prerequisites
Before running the application, ensure you have the following prerequisites installed on your system:

.NET 6 SDK
Entity Framework Core
Hangfire
Swashbuckle.AspNetCore (Swagger)
Installation and Setup

1. Clone the repository:
   git clone https://github.com/your-username/restaurant-ordering-system.git

2. Navigate to the project directory:
   cd restaurant-ordering-system

3. Build the project:
   dotnet build
   
5. Run the project:
   dotnet run

API Endpoints
The following API endpoints are available:

Menu API

- GET /api/menu: Retrieve all menu items.
- GET /api/menu/{id}: Retrieve a specific menu item by ID.
- POST /api/menu: Create a new menu item.
- PUT /api/menu/{id}: Update a menu item.
- DELETE /api/menu/{id}: Delete a menu item.

Orders API

- GET /api/orders: Retrieve all orders.
- GET /api/orders/{id}: Retrieve a specific order by ID.
- POST /api/orders: Create a new order.
- PUT /api/orders/{id}: Update an order.
- DELETE /api/orders/{id}: Delete an order.

Swagger Documentation
The project is integrated with Swagger for API documentation. You can access the Swagger UI by navigating to https://localhost:5001/swagger in your browser. The Swagger UI provides detailed information about the available endpoints, request/response models, and allows you to interact with the APIs.

Database Configuration
The project is currently configured to use an in-memory database for development purposes. If you want to use a different database, you can modify the database connection string in the appsettings.json file.

Logging
The project utilizes logging to capture important events and errors. The log files are stored in the Logs directory and are named based on the date.

