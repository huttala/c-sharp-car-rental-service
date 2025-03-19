using CarRental.Application.DTOs.Booking;
using CarRental.Application.DTOs.Vehicle;
using CarRental.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Endpoints;

public static class BookingEndpoints
{
    public static WebApplication MapBookingEndpoints(this WebApplication app)
    {
        // Get all bookings
        app.MapGet("/bookings", async ([FromServices] IBookingService bookingService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var bookings = await bookingService.GetAllBookings();
                    return Results.Ok(bookings);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to retrieve bookings",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status404NotFound);
                }
            })
            .WithName("GetAllBookings")
            .WithOpenApi()
            .WithTags("Bookings");
        // Get booking by Id
        app.MapGet("/bookings/{id}", async (
                Guid id,
                [FromServices] IBookingService bookingService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var booking = await bookingService.GetBookingById(id);
                    return Results.Ok(booking);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to retrieve booking",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status404NotFound);
                }
            })
            .WithName("GetBookingById")
            .WithOpenApi()
            .WithTags("Bookings");
        // Create booking
        app.MapPost("/bookings", async (
                [FromBody] CreateBookingDTO createDto,
                [FromServices] IBookingService bookingService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var createdBooking = await bookingService.CreateBooking(createDto);
                    return Results.Ok(createdBooking);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to create booking",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("CreateBooking")
            .WithOpenApi()
            .WithTags("Bookings");
        // Finalize booking
        app.MapPut("/bookings/{id}/finalize", async (
                Guid id,
                [FromBody] FinalizeBookingDTO finalizeDto,
                [FromServices] IBookingService bookingService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    // Ensure the ID in the route matches the DTO
                    if (id != finalizeDto.BookingId)
                        return Results.BadRequest("ID mismatch between route and body");
                
                    var finalizedBooking = await bookingService.FinalizeBooking(finalizeDto);
                    return Results.Ok(finalizedBooking);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to finalize booking",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("FinalizeBooking")
            .WithOpenApi()
            .WithTags("Bookings");
        // Delete booking
        app.MapDelete("/bookings/{id}", async (
                Guid id,
                [FromServices] IBookingService bookingService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    await bookingService.DeleteBooking(id);
                    return Results.Ok();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to finalize booking",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("DeleteBooking")
            .WithOpenApi()
            .WithTags("Bookings");
        return app;
    }
}