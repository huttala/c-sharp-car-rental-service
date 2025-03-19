namespace CarRental.Application.DTOs.Booking;

public class FinalizeBookingDTO
{
    public required Guid BookingId { get; init; }
    public required uint ReturnedMilage { get; init; }
}