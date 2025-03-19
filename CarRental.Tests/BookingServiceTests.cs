using CarRental.Application.Configuration;
using CarRental.Application.DTOs.Booking;
using CarRental.Application.Services;
using CarRental.Domain.Enums;
using CarRental.Domain.Model;
using CarRental.Domain.Repositories;
using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace CarRental.Tests;

public class BookingServiceTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<IVehicleRepository> _vehicleRepoMock;
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly Mock<ICustomerRepository> _customerRepoMock;
    private readonly BookingService _bookingService;
    private readonly Mock<IOptions<PricingConfiguration>> _pricingConfigMock;
    
    public BookingServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _vehicleRepoMock = new Mock<IVehicleRepository>();
        _bookingRepoMock = new Mock<IBookingRepository>();
        _customerRepoMock = new Mock<ICustomerRepository>();
        
        // Create and set up your pricing config mock
        _pricingConfigMock = new Mock<IOptions<PricingConfiguration>>();
        _pricingConfigMock.Setup(x => x.Value).Returns(new PricingConfiguration
        {
            BaseDailyPrice = 600,
            BaseKmPrice = 20,
            Multipliers = new PricingMultipliers
            {
                SmallCar = new VehicleTypeMultiplier { DailyRate = 1.0m, KmRate = 0.0m },
                CombiCar = new VehicleTypeMultiplier { DailyRate = 1.3m, KmRate = 1.0m },
                Truck = new VehicleTypeMultiplier { DailyRate = 1.5m, KmRate = 1.5m }
            }
        });
        
        _bookingService = new BookingService(
            _vehicleRepoMock.Object,
            _bookingRepoMock.Object,
            _customerRepoMock.Object,
            _pricingConfigMock.Object);
    }
    
    [Fact]
    public async Task CreateBooking_WithValidData_ReturnsCreatedBooking()
    {
        // Arrange

        var customerPnr = "192012120000";
        var vehiclePlateNumber = "ABC123";
        var customer = new Customer(customerPnr, "John", "Doe", "john@example.com", "1234567890");
        var vehicle = new Vehicle(vehiclePlateNumber, VehicleType.SmallCar, 1000);
        
        var bookingNumber = "ABC123-12345-ABCDE";
        var createdBooking = new Booking(bookingNumber, vehicle.Id, customer.Id, DateTime.UtcNow, 1000);
        
        _customerRepoMock.Setup(x => x.GetByPersonalNumberAsync(customerPnr))
            .ReturnsAsync(customer);
            
        _vehicleRepoMock.Setup(x => x.GetByLicencePlateAsync(vehiclePlateNumber))
            .ReturnsAsync(vehicle);
            
        _bookingRepoMock.Setup(x => x.CreateBookingAsync(It.IsAny<Booking>(), vehicle))
            .Returns(Task.CompletedTask);
            
        _bookingRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(createdBooking);
        
        var createBookingDto = new CreateBookingDTO 
        { 
            LicencePlate = vehiclePlateNumber,
            CustomerPersonalNumber = customerPnr
        };
        
        // Act
        var result = await _bookingService.CreateBooking(createBookingDto);
        
        // Assert
        result.Should().NotBeNull();
        result.VehicleId.Should().Be(vehicle.Id);
        result.CustomerId.Should().Be(customer.Id);
        result.BookingNumber.Should().StartWith(vehiclePlateNumber);
        
        _bookingRepoMock.Verify(x => x.CreateBookingAsync(It.IsAny<Booking>(), vehicle), Times.Once);
    }
    
    [Fact]
    public async Task CreateBooking_VehicleNotAvailable_ThrowsArgumentException()
    {
        // Arrange
        var customer = new Customer("192012120000", "John", "Doe", "john@example.com", "1234567890");
        var vehicle = new Vehicle("ABC123", VehicleType.SmallCar, 1000);
        vehicle.UpdateVehicleStatus(VehicleStatus.Unavailable);
        
        var createDto = new CreateBookingDTO 
        { 
            LicencePlate = "ABC123",
            CustomerPersonalNumber = "192012120000"
        };
        
        _customerRepoMock.Setup(x => x.GetByPersonalNumberAsync("192012120000"))
            .ReturnsAsync(customer);
            
        _vehicleRepoMock.Setup(x => x.GetByLicencePlateAsync("ABC123"))
            .ReturnsAsync(vehicle);
            
        // Act & Assert
        await _bookingService.Invoking(s => s.CreateBooking(createDto))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Vehicle is not available");
    }
    
    [Fact]
    public async Task FinalizeBooking_ValidData_ReturnsFinalizedBooking()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        
    
        var vehicle = new Vehicle("ABC123", VehicleType.SmallCar, 1000);
        var initialMileage = 1000u;
        var returnedMileage = 1500u;
    
        var booking = new Booking("ABC123-12345-ABCDE", vehicleId, customerId, DateTime.UtcNow, initialMileage);
        var bookingId = booking.Id;
        var finalizeDto = new FinalizeBookingDTO
        {
            BookingId = bookingId,
            ReturnedMilage = returnedMileage
        };
    
        _bookingRepoMock.Setup(x => x.GetByIdAsync(bookingId))
            .ReturnsAsync(booking);
        
        _vehicleRepoMock.Setup(x => x.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);
        
        _bookingRepoMock.Setup(x => x.FinalizeBookingAsync(booking, vehicle))
            .Returns(Task.CompletedTask);
    
        // Act
        var result = await _bookingService.FinalizeBooking(finalizeDto);
    
        // Assert
        result.Should().NotBeNull();
        result.ReturnedVehicleMileage.Should().Be(returnedMileage);
        result.TotalCost.Should().BeGreaterThan(0); // With dynamic pricing (which we don't have now) this should be calculated. 
    
        _bookingRepoMock.Verify(x => x.FinalizeBookingAsync(booking, vehicle), Times.Once);
        _vehicleRepoMock.Verify(x => x.GetByIdAsync(vehicleId), Times.Once);
    }

    [Fact]
    public async Task FinalizeBooking_BookingNotFound_ThrowsException()
    {
        // Arrange
        var finalizeDto = new FinalizeBookingDTO
        {
            BookingId = Guid.NewGuid(),
            ReturnedMilage = 1500
        };
    
        _bookingRepoMock.Setup(x => x.GetByIdAsync(finalizeDto.BookingId))
            .ReturnsAsync((Booking)null);
    
        // Act & Assert
        await _bookingService.Invoking(s => s.FinalizeBooking(finalizeDto))
            .Should().ThrowAsync<Exception>()
            .WithMessage("Booking not found");
    }
    
    [Fact]
    public void CalculateTotalCost_SmallCar_CalculatesCorrectly()
    {
        // Arrange
        
        // NOTE: Since the daily count is based on time as well, oneDayAgo + the millisecond this test takes will sometimes return 3 days
        // which makes the test flaky. 
        // That is why we add one hour to the booking datetime.
        var booking = new Booking("TEST-123", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-2).AddHours(1), 1000);
        booking.ReturnVehicle(1200); // 200km traveled
        
        var vehicle = new Vehicle("TEST", VehicleType.SmallCar, 1000);
        
        var daysRented = 2;
        var expectedCost = _pricingConfigMock.Object.Value.BaseDailyPrice * daysRented * 
                           _pricingConfigMock.Object.Value.Multipliers.SmallCar.DailyRate;

        
        // Act
        var totalCost = _bookingService.CalculateTotalCost(booking, vehicle);
        
        // Assert
        // Base daily price 600 * 2 days = 1200 
        totalCost.Should().Be(expectedCost);
    }

    [Fact]
    public void CalculateTotalCost_CombiCar_CalculatesCorrectly()
    {
        // Arrange
        var booking = new Booking("TEST-123", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-3).AddHours(1), 1000);
        booking.ReturnVehicle(1300); // 300km traveled
        
        var vehicle = new Vehicle("TEST", VehicleType.CombiCar, 1000);
        
        var daysRented = 3;
        var kmTraveled = 300;
        var expectedCost = (_pricingConfigMock.Object.Value.BaseDailyPrice * daysRented * 
                            _pricingConfigMock.Object.Value.Multipliers.CombiCar.DailyRate) +
                           (_pricingConfigMock.Object.Value.BaseKmPrice * kmTraveled * 
                            _pricingConfigMock.Object.Value.Multipliers.CombiCar.KmRate);

        
        // Act
        var totalCost = _bookingService.CalculateTotalCost(booking, vehicle);
        
        // Assert
        // (600 * 3 days * 1.3) + (20 * 300km) = 2340 + 6000 = 8340
        totalCost.Should().Be(expectedCost);
    }

    [Fact]
    public void CalculateTotalCost_Truck_CalculatesCorrectly()
    {
        // Arrange
        var booking = new Booking("TEST-123", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-2).AddHours(1), 1000);
        booking.ReturnVehicle(1100); // 100km traveled
        
        var vehicle = new Vehicle("TEST", VehicleType.Truck, 1000);
        
        var daysRented = 2;
        var kmTraveled = 100;
        var expectedCost = (_pricingConfigMock.Object.Value.BaseDailyPrice * daysRented * 
                            _pricingConfigMock.Object.Value.Multipliers.Truck.DailyRate) +
                           (_pricingConfigMock.Object.Value.BaseKmPrice * kmTraveled * 
                            _pricingConfigMock.Object.Value.Multipliers.Truck.KmRate);

        
        // Act
        var totalCost = _bookingService.CalculateTotalCost(booking, vehicle);
        
        // Assert
        // (600 * 2 days * 1.5) + (20 * 100km * 1.5) = 1800 + 3000 = 4800
        totalCost.Should().Be(expectedCost);
    }

    [Fact]
    public void GenerateBookingNumber_ReturnsValidFormat()
    {
        // Arrange
        var licensePlate = "ABC123";
        
        // Act
        var bookingNumber = BookingService.GenerateBookingNumber(licensePlate);
        
        // Assert
        bookingNumber.Should().StartWith(licensePlate + "-");
        var parts = bookingNumber.Split('-');
        parts.Should().HaveCount(3);
        
        // Middle part should be a number between 10000-99999
        int.TryParse(parts[1], out int middlePart).Should().BeTrue();
        middlePart.Should().BeGreaterThanOrEqualTo(10000);
        middlePart.Should().BeLessThan(100000);
        
        // Last part should be 5 uppercase letters
        parts[2].Length.Should().Be(5);
        parts[2].All(char.IsUpper).Should().BeTrue();
    }
    
    
    // Property based test runs 1000 iterations of randomized data according to spec.
    // This will also test the VehicleService.ValidLicensePlate method.
    // But right now only Booking tests exist.
    [Property(MaxTest = 1000)]
    public Property GenerateBookingNumber_AlwaysFollowsExpectedFormat()
    {
        // Generate a valid license plate
        var letters = Gen.Elements('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z');
        var digits = Gen.Elements('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
    
        var validPlateGen = 
            from l1 in letters
            from l2 in letters
            from l3 in letters
            from d1 in digits
            from d2 in digits
            from d3 in digits
            select new string(new[] {l1, l2, l3, d1, d2, d3});
    
        return Prop.ForAll(validPlateGen.ToArbitrary(), licensePlate => {
            // Act
            var bookingNumber = BookingService.GenerateBookingNumber(licensePlate);
            // _testOutputHelper.WriteLine(licensePlate);
            // Assert properties
            var parts = bookingNumber.Split('-');
        
            return 
                parts.Length == 3 &&
                bookingNumber.StartsWith(licensePlate + "-") &&
                int.TryParse(parts[1], out int num) && num >= 10000 && num < 100000 &&
                parts[2].Length == 5 && parts[2].All(char.IsUpper);
        });
    }
    
    
    
}