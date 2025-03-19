using CarRental.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.DbConfiguration.EntityConfiguration;

public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
{

    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PersonalNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.FirstName).HasMaxLength(50);
        builder.Property(x => x.LastName).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.PhoneNumber).HasMaxLength(50);
        
        // Indexes
        builder.HasIndex(x => x.PersonalNumber).IsUnique();
        
        // Concurrency fail safe
        builder.Property(x => x.RowVersion)
            .HasDefaultValueSql("'\\x0000000000000001'::bytea") // Needed for postgres to handle row versions using EF Core
            .IsRowVersion();
        
        builder.HasMany(c => c.Bookings)
            .WithOne()
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.SetNull); // No traceability of customers in case of GDPR wipe. 
    }
    
}