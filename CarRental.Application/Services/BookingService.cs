using System.Reflection.Metadata.Ecma335;
using CarRental.Application.DTOs.Booking;
using CarRental.Application.DTOs.Vehicle;
using CarRental.Application.Mappings.BookingMappings;
using CarRental.Domain.Enums;
using CarRental.Domain.Model;
using CarRental.Domain.Repositories;

namespace CarRental.Application.Services;

public interface IBookingService
{
    Task<BookingDTO> GetBookingById(Guid id);
    Task<List<BookingDTO>> GetAllBookings();
    Task<BookingDTO> CreateBooking(CreateBookingDTO booking);
    Task<BookingDTO> FinalizeBooking(FinalizeBookingDTO booking);
    Task DeleteBooking(Guid id);
}


public class BookingService : IBookingService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ICustomerRepository _customerRepository;
    private static readonly Random _random = new Random();
    
    public BookingService(IVehicleRepository vehicleRepository, 
        IBookingRepository bookingRepository,
        ICustomerRepository customerRepository
        )
    {
        _vehicleRepository = vehicleRepository;
        _bookingRepository = bookingRepository;
        _customerRepository = customerRepository;
    }
    
    public async Task<BookingDTO> GetBookingById(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if  (booking == null)
            throw  new KeyNotFoundException($"Booking with id {id} not found");
        return BookingMapper.ToDTO(booking);
    }

    public async Task<List<BookingDTO>> GetAllBookings()
    {
        var bookings = await _bookingRepository.GetAllAsync();
        return bookings.Select(b => BookingMapper.ToDTO(b)).ToList();
    }

    public async Task<BookingDTO> CreateBooking(CreateBookingDTO booking)
    {
        var customer = await _customerRepository.GetByPersonalNumberAsync(booking.CustomerPersonalNumber);
        var vehicle = await _vehicleRepository.GetByLicencePlateAsync(booking.LicencePlate);
        if (customer == null) 
            throw new ArgumentException("Customer not found");
        if  (vehicle == null)
            throw new ArgumentException("Vehicle not found");
        if (vehicle.VehicleStatus != VehicleStatus.Available)
            throw new ArgumentException("Vehicle is not available");

        var bookingNr = GenerateBookingNumber(vehicle.LicencePlate);
        var bookingToCreate = new Booking(bookingNr, vehicle.Id, customer.Id, DateTime.UtcNow, vehicle.Mileage);
        try
        {
            vehicle.UpdateVehicleStatus(VehicleStatus.Unavailable);
            await _bookingRepository.CreateBookingAsync(bookingToCreate, vehicle);
            var createdBooking = await _bookingRepository.GetByIdAsync(bookingToCreate.Id);
            if (createdBooking == null)
                throw new Exception("Could not create booking");
            return BookingMapper.ToDTO(createdBooking);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<BookingDTO> FinalizeBooking(FinalizeBookingDTO dto)
    {
        var booking = await _bookingRepository.GetByIdAsync(dto.BookingId);
        if (booking == null)
            throw new Exception("Booking not found");
        
        var vehicleUsedInBooking = await _vehicleRepository.GetByIdAsync(booking.VehicleId);
        if (vehicleUsedInBooking == null)
            throw new Exception("Vehicle not found when finalizing booking");
        
        vehicleUsedInBooking.UpdateVehicleStatus(VehicleStatus.Available);
        vehicleUsedInBooking.UpdateMilage(dto.ReturnedMilage);
        
        booking.ReturnVehicle(dto.ReturnedMilage);
        var totalCost = CalculateTotalCost(booking, vehicleUsedInBooking);
        booking.AddTotalCost(totalCost);
        
        await _bookingRepository.FinalizeBookingAsync(booking, vehicleUsedInBooking);
        
        return BookingMapper.ToDTO(booking);
    }
    
    public async Task DeleteBooking(Guid id)
    {
        // if booking does not exist, just return
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return;
        
        // Make sure the vehicle connected to this booking is set to available agian.
        var vehicle = await _vehicleRepository.GetByIdAsync(booking.VehicleId);
        vehicle?.UpdateVehicleStatus(VehicleStatus.Available);
        await _bookingRepository.DeleteBookingAsync(booking, vehicle);
    }
    
    public static string GenerateBookingNumber(string licensePlate)
    {
        if (!VehicleService.ValidLicensePlate(licensePlate))
        {
            throw new ArgumentException($"Invalid license plate '{licensePlate}'");
        }
        var randomNumber = _random.Next(10000, 100000);
        var randomChars = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 5)
            .Select(s => s[_random.Next(s.Length)])
            .ToArray());

        return $"{licensePlate}-{randomNumber}-{randomChars}";
    }
    
    public static decimal CalculateTotalCost(Booking booking,  Vehicle vehicle)
    {
        if (booking.ReturnedVehicleMileage == null)
            throw new ArgumentException("ReturnedVehicleMileage cannot be null");
        if (booking.ReturnedDateTime == null)
            throw new ArgumentException("ReturnedDateTime cannot be null");
        
        // Base rates (just took some since there was none specified)
        decimal baseDailyPrice = 600;
        decimal baseKmPrice = 20;
        
        var traveledDistance = booking.ReturnedVehicleMileage.Value - booking.BookedVehicleMileage;
    
        // Calculate days rented (round up to full days)
        var daysRented = (int)Math.Ceiling((booking.ReturnedDateTime.Value - booking.BookedDateTime).TotalDays);
        daysRented = daysRented < 1 ? 1 : daysRented; // Minimum 1 day
    
        // Calculate cost based on vehicle type
        decimal totalCost = 0;
    
        switch (vehicle.VehicleType)
        {
            case VehicleType.SmallCar:
                totalCost = baseDailyPrice * daysRented;
                break;
            
            case VehicleType.CombiCar:
                totalCost = (baseDailyPrice * daysRented * 1.3m) + (baseKmPrice * traveledDistance);
                break;
            
            case VehicleType.Truck:
                totalCost = (baseDailyPrice * daysRented * 1.5m) + (baseKmPrice * traveledDistance * 1.5m);
                break;
            
            default:
                throw new ArgumentException($"Unsupported vehicle type: {vehicle.VehicleType}");
        }
    
        return Math.Round(totalCost, 2); // Round to 2 decimal places
    }
    
}