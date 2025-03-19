using CarRental.Application.DTOs.Customer;
using CarRental.Domain.Model;

namespace CarRental.Application.Mappings.CustomerMappings;

public static class CustomerMapper
{
    public static CustomerDTO ToDTO(this Customer customer)
    {
        return new CustomerDTO
        {
            Id = customer.Id,
            PersonalNumber = customer.PersonalNumber,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber
        };
    }
}