using System.Data;
using CarRental.Domain.Enums;
using CarRental.Domain.Model;
using CarRental.Domain.Repositories;
using CarRental.Infrastructure.DbConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Persistence;

public class BookingRepository : IBookingRepository
{
    private readonly CarRentalContext _context;

    public BookingRepository(CarRentalContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings.ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings.FindAsync(id);
    }

    public async Task<Booking?> GetByBookingNumberAsync(string bookingNumber)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
    }

    public async Task CreateBookingAsync(Booking booking, Vehicle vehicle)
    {
        // Since double bookings should not exist, make this transaction serializable.
        // If there is performance issues you could just use RowVersions or similar with less restricted isolationlevels.
       
        var strategy = _context.Database.CreateExecutionStrategy();
    
        // Execute the transaction using the strategy
        await strategy.ExecuteAsync(async () =>
        {
            // Since double bookings should not exist, make this transaction serializable.
            await using var transaction = 
                await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                await _context.Bookings.AddAsync(booking);
                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();
        
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new Exception("Could not add booking, try again");
            }
        });
    }

    public async Task FinalizeBookingAsync(Booking booking, Vehicle vehicle)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
    
        await strategy.ExecuteAsync(async () =>
        {
            // ReadCommitted is sufficient here since the vehicle is already marked as booked
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                _context.Vehicles.Update(vehicle);
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
            
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to finalize booking", ex);
            }
        });
    }
    
    public async Task DeleteBookingAsync(Booking booking,  Vehicle? vehicle)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            try
            {
                if (vehicle != null)
                    _context.Vehicles.Update(vehicle);
                _context.Bookings.Remove(booking);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new Exception("Could not delete booking, try again");
            }
        });
    }
}