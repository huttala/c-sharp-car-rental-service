using CarRental.Application.DTOs.Booking;
using CarRental.Application.DTOs.Customer;
using CarRental.Domain.Model;

namespace CarRental.Application.Mappings.BookingMappings;

public static class BookingMapper
{
    public static BookingDTO ToDTO(Booking booking)
    {
        return new BookingDTO
        {
            Id = booking.Id,
            BookingNumber = booking.BookingNumber,
            VehicleId = booking.VehicleId,
            CustomerId = booking.CustomerId,
            BookedDateTime = booking.BookedDateTime,
            ReturnedDateTime = booking.ReturnedDateTime,
            BookedVehicleMileage = booking.BookedVehicleMileage,
            ReturnedVehicleMileage = booking.ReturnedVehicleMileage,
            TotalCost = booking.TotalCost
        };
    }
}