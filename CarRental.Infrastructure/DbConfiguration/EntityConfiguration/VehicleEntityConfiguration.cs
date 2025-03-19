using CarRental.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbConfiguration.EntityConfiguration;

public class VehicleEntityConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LicencePlate).HasMaxLength(25).IsRequired();
        builder.Property(x => x.VehicleType).IsRequired();
        builder.Property(x => x.Mileage).IsRequired();
        builder.Property(x => x.Deleted).IsRequired();
       
        // Make sure the LicencePlate cannot be duplicated, since it creates a index, searching by LicendePlate will 
        // be more performant as well. 
        builder.HasIndex(x => x.LicencePlate).IsUnique();

        
        // Concurrency fail safe
        builder.Property(x => x.RowVersion)
            .HasDefaultValueSql("'\\x0000000000000001'::bytea") // Needed for postgres to handle row versions using EF Core
            .IsRowVersion();
        
        // Add composite index on VehicleStatus and Type for faster queries of available vehicles.
        builder.HasIndex(x => new { x.VehicleStatus, x.VehicleType });
    }
}