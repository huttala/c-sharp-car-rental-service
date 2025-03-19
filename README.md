# Car Rental Application

A car rental management system built with .NET 9 and PostgreSQL.

## Quick Start

### Development Environment

This project uses Dev Containers for a consistent development environment:

1. **Start the Dev Container**:
   - In your IDE (Visual Studio, Rider, or VS Code), right-click on the `.devcontainer` folder or file
   - Select "Start Dev Container" or equivalent option in your IDE
   - Wait for the container to build and start

2. **Database**:
   - The Dev Container includes a PostgreSQL database instance
   - Connection details:
     - Host: localhost
     - Port: 1337
     - Username: postgres
     - Password: postgres
     - Database: car-rental-dev
   - The database is automatically seeded with sample vehicles and a customer when you run the application in development mode

### Running the Application

1. Build and run the application from your IDE
2. Access the Swagger UI at: `http://localhost:5160`

### Using the Application

#### Create a Booking

```json
POST /bookings
{
  "licencePlate": "ABC123",
  "customerPersonalNumber": "192012120000"
}
```

#### Finalize a Booking

```json
PUT /bookings/{bookingId}/finalize
{
  "bookingId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "returnedMilage": 1500
}
```

The system will calculate the total cost based on vehicle type, rental duration, and distance traveled.