# .NET E-Commerce Solution

A comprehensive .NET 9.0 e-commerce solution with multiple sub-projects focusing on different aspects of an online shopping system. This project demonstrates console-based UI with Terminal.GUI, Entity Framework Core with SQLite, and modular architecture.

## Project Structure

The solution contains the following main components:

### App (Main Application)
The core e-commerce application with full functionality including user authentication, product management, shopping cart, and order processing.

[View App Details](./App/README.md)

### Project1 (Shape Calculator)
A utility application for geometric calculations and shape management.

[View Project1 Details](./project1/README.md)

### Project2 (Product Management System)
A focused product and category management system with user authentication.

[View Project2 Details](./project2/README.md)

## Technologies Used

- **.NET 9.0**: Latest .NET framework
- **Entity Framework Core**: For database operations with SQLite
- **Terminal.GUI**: For text-based user interface
- **Docker**: Containerization support
- **Spectre.Console**: Enhanced console UI components
- **Figgle**: ASCII art text generation

## Getting Started

### Prerequisites
- .NET 9.0 SDK or later
- Docker (optional, for containerized deployment)

### Running the Application

1. Clone the repository
2. Navigate to the project root
3. Run the main application:
   ```
   cd App
   dotnet run
   ```

## Screenshots

The `images` directory contains application screenshots demonstrating different application features:

- User authentication
- Product management
- Shopping cart functionality
- Order processing
- Reports and analytics

## Docker Support

All projects include Docker support for containerized deployment. Use the included compose.yaml file to run the full solution:

```
docker-compose up
```

## Database Migrations

The solution uses Entity Framework Core migrations to manage the database schema. Migration files are included in each project's Migrations folder.

## Project Structure Details

Each sub-project includes:
- Models for data representation
- Services for business logic
- Pages for UI components
- Database context and migrations