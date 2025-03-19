using CarRental.Application.DTOs.Vehicle;
using CarRental.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Endpoints;

public static class VehicleEndpoints
{
    public static WebApplication MapVehicleEndpoints(this WebApplication app)
    {
        // Get all vehicles
        app.MapGet("/vehicles", async ([FromServices] IVehicleService vehicleService,
                                              CancellationToken cancellationToken) =>
            {
                try
                {
                    var vehicles = await vehicleService.GetAllVehicles();
                    return Results.Ok(vehicles);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to retrieve vehicles",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status404NotFound);
                }
            })
            .WithName("GetAllVehicles")
            .WithOpenApi()
            .WithTags("Vehicles");
        
        // Get vehicle by Id
        app.MapGet("/vehicles/{id}", async (
                Guid id,
                [FromServices] IVehicleService vehicleService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var vehicle = await vehicleService.GetVehicleById(id);
                    return Results.Ok(vehicle);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to retrieve vehicles",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status404NotFound);
                }
            })
            .WithName("GetVehicleById")
            .WithOpenApi()
            .WithTags("Vehicles");
        
        // Create vehicle
        app.MapPost("/vehicles", async (
            [FromBody] CreateVehicleDTO createDto,
            [FromServices] IVehicleService vehicleService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var createdVehicle = await vehicleService.AddVehicle(createDto);
                return Results.Ok(createdVehicle);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("license plate"))
            {
                return Results.Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to create vehicle",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("CreateVehicle")
        .WithOpenApi()
        .WithTags("Vehicles");
        
        // Delete vehicle
        app.MapDelete("/vehicles/{id}", async (
                Guid id,
                [FromServices] IVehicleService vehicleService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    await vehicleService.DeleteVehicle(id);
                    return Results.Ok();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to finalize vehicle",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("DeleteVehicle")
            .WithOpenApi()
            .WithTags("Vehicles");
        
        
        return app;
    }
}