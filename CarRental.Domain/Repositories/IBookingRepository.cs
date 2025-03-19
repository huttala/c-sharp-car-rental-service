using CarRental.Domain.Model;

namespace CarRental.Domain.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(Guid id);
    Task CreateBookingAsync(Booking booking, Vehicle vehicle);
    Task FinalizeBookingAsync(Booking booking, Vehicle vehicle);
    Task DeleteBookingAsync(Booking booking, Vehicle vehicle);
}