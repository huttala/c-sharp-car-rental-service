using CarRental.Domain.Model;

namespace CarRental.Domain.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByPersonalNumberAsync(string personalNumber);
    Task<List<Customer>> GetByNameAsync(string searchTerm);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task<List<Booking>> GetCustomerBookingsAsync(Guid customerId);
    Task DeleteCustomerAsync(Guid customerId);
}