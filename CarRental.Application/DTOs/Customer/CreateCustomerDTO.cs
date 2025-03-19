namespace CarRental.Application.DTOs.Customer;

public class CreateCustomerDTO
{
    public required string PersonalNumber { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Email { get; init; } = null; 
    public string? PhoneNumber { get; init; } = null; 
}