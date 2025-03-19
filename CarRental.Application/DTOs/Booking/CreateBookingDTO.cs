namespace CarRental.Application.DTOs.Booking;

public class CreateBookingDTO
{
    public required string LicencePlate { get; init; }
    public required string CustomerPersonalNumber { get; init; }
}