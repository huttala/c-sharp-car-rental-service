namespace CarRental.Application.DTOs.Customer;

public class CustomerDTO
{
    public required Guid Id { get; init; }
    public required string PersonalNumber { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
}