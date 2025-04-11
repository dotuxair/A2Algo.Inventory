# A2Algo.Inventory

## API for Inventory Management

This API is designed to handle inventory-related tasks, including:

* Creating, updating, and deleting products.
* Managing sales and purchases of products.

## Development Guide

This guide provides instructions for setting up and running the A2Algo.Inventory API.

### Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.
* A database system (e.g., SQL Server, PostgreSQL) configured and running.

### Setup Instructions

1.  **Clone the Repository:**
    ```bash
    git clone <repository_url>
    cd A2Algo.Inventory
    ```
    *(Replace `<repository_url>` with the actual URL of your repository)*

2.  **Clean and Build the Project:**
    ```bash
    dotnet clean
    dotnet build
    ```
    This command will clean any previous build outputs and then build the project. Ensure there are no build errors before proceeding.

3.  **Configure Connection String:**
    Locate the application configuration file (e.g., `appsettings.json`) in the project directory. Modify the connection string under the appropriate section (e.g., `ConnectionStrings`) to point to your database instance.

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;TrustServerCertificate=true;"
      },
      // ... other configurations
    }
    ```

    *(Replace `your_server`, `your_database`, `your_user`, and `your_password` with your database credentials and server details.)*

4.  **Apply Migrations:**
    This step ensures that your database schema is up-to-date with the Entity Framework Core models defined in the project.

    ```bash
    dotnet ef database update
    ```

    If you encounter issues, you might need to add migrations first:

    ```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

    *(Replace `InitialCreate` with a descriptive name for your initial migration if you haven't run migrations before.)*

5.  **Run the API:**
    ```bash
    dotnet run
    ```
    This command will build and run the API. You should see output indicating the API is listening on a specific port (usually `http://localhost:5xxx`).

### Using the API

Once the API is running, you can interact with its endpoints using tools like Postman, Swagger UI (if configured), or by making HTTP requests from your application.

**Base URL:** `http://localhost:5xxx` *(Replace `5xxx` with the actual port your API is running on)*
