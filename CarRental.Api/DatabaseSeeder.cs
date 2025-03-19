using CarRental.Domain.Enums;
using CarRental.Domain.Model;
using CarRental.Infrastructure.DbConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Api;

public static class DatabaseSeeder
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CarRentalContext>();

        // Only seed if database is empty
        if (!await context.Vehicles.AnyAsync() && !await context.Customers.AnyAsync())
        {
            await SeedVehicles(context);
            await SeedCustomers(context);
            
            await context.SaveChangesAsync();
        }
    }
    
    private static async Task SeedVehicles(CarRentalContext context)
    {
        var vehicles = new List<Vehicle>
        {
            new Vehicle("ABC123", VehicleType.SmallCar, 0),
            new Vehicle("ABC124", VehicleType.CombiCar, 0),
            new Vehicle("ABC125", VehicleType.Truck, 0)
        };
        
        await context.Vehicles.AddRangeAsync(vehicles);
    }
    
    private static async Task SeedCustomers(CarRentalContext context)
    {
        var customer = new Customer("192012120000", "Niklas", "Huhtala", "niklas.huhtala@activesolution.se", "0760220078");
        
        await context.Customers.AddAsync(customer);
    }
}