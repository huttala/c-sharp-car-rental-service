using CarRental.Application.DTOs.Customer;
using CarRental.Application.DTOs.Vehicle;
using CarRental.Application.Mappings.CustomerMappings;
using CarRental.Domain.Model;
using CarRental.Domain.Repositories;

namespace CarRental.Application.Services;

public interface ICustomerService
{
    Task<CustomerDTO> GetCustomerById(Guid id);
    Task<List<CustomerDTO>> GetAllCustomers();
    Task<CustomerDTO> AddCustomer(CreateCustomerDTO customer);
    Task DeleteCustomer(Guid id);
}

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    
    public async Task<CustomerDTO> GetCustomerById(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            throw new ArgumentException("Customer not found", nameof(id));
        
        var dto = CustomerMapper.ToDTO(customer);

        return dto;

    }

    public async Task<List<CustomerDTO>> GetAllCustomers()
    {
        var customers = await _customerRepository.GetAllAsync();
        var customerDtos = customers.Select(customer => CustomerMapper.ToDTO(customer)).ToList();
        return customerDtos;
    }

    public async Task<CustomerDTO> AddCustomer(CreateCustomerDTO dto)
    {
        var existingCustomer = await _customerRepository.GetByPersonalNumberAsync(dto.PersonalNumber);
        if (existingCustomer != null)
            throw new ArgumentException("Customer already exists");
        
        var customer = new Customer(dto.PersonalNumber, dto.FirstName, dto.LastName, dto.Email, dto.PhoneNumber);
        await _customerRepository.AddAsync(customer);
        return CustomerMapper.ToDTO(customer);
    }

    public async Task DeleteCustomer(Guid id)
    {
        await _customerRepository.DeleteCustomerAsync(id);
    }
}