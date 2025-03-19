namespace CarRental.Application.DTOs.Booking;

public class BookingDTO
{
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required string BookingNumber { get; init; }
    public required Guid VehicleId { get; init; }
    public Guid? CustomerId { get; init; } 
    public required DateTime BookedDateTime { get; init; }
    public DateTime? ReturnedDateTime { get; init; }
    public required uint BookedVehicleMileage { get; init; }
    public uint? ReturnedVehicleMileage { get; init; } 
    public decimal? TotalCost { get; init; }
}