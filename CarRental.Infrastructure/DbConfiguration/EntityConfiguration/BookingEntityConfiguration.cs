using CarRental.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbConfiguration.EntityConfiguration;

public class BookingEntityConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.VehicleId).IsRequired();
        builder.Property(x => x.CustomerId);
        builder.Property(x => x.BookedDateTime).IsRequired();
        builder.Property(x => x.ReturnedDateTime);
        builder.Property(x => x.BookedVehicleMileage).IsRequired();
        builder.Property(x => x.ReturnedVehicleMileage);
        
        // Concurrency fail safe
        builder.Property(x => x.RowVersion)
            .HasDefaultValueSql("'\\x0000000000000001'::bytea") // Needed for postgres to handle row versions using EF Core
            .IsRowVersion();
        
        builder.HasOne<Customer>()
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CustomerId)
            .IsRequired(false) // FK Should be nullable in case of GDPR wipe of the customer.
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(b => b.VehicleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); // Restrict deletion when linked vehicle exists (Should always exist since they are soft deleted).
    }
}