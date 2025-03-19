using CarRental.Domain.Model;
using CarRental.Domain.Repositories;
using CarRental.Infrastructure.DbConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Persistence;

public class CustomerRepository : ICustomerRepository
{
        private readonly CarRentalContext _context;

    public CustomerRepository(CarRentalContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<Customer?> GetByPersonalNumberAsync(string personalNumber)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.PersonalNumber == personalNumber);
    }

    public async Task<List<Customer>> GetByNameAsync(string searchTerm)
    {
        return await _context.Customers
            .Where(c => c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Booking>> GetCustomerBookingsAsync(Guid customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == customerId);

        return customer?.Bookings.ToList() ?? new List<Booking>();
    }

    public async Task DeleteCustomerAsync(Guid customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            // DeleteCustomer will soft-delete customer so Bookings can keep their relations.
            // All GDPR sensitive data will also be removed.
            customer.DeleteCustomer();
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}