using CarRental.Domain.Model;
using CarRental.Infrastructure.DbConfiguration.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.DbConfiguration;

public class CarRentalContext : DbContext
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options)
    {
        // Just create the DB. No need for migrations since this is a POC.  
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VehicleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BookingEntityConfiguration());
    }
}