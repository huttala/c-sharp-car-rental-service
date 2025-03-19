using CarRental.Application.DTOs.Customer;
using CarRental.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Endpoints;

public static class CustomerEndpoints
{
    public static WebApplication MapCustomerEndpoints(this WebApplication app)
    {
        // Get all customers
        app.MapGet("/customers", async ([FromServices] ICustomerService customerService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var customers = await customerService.GetAllCustomers();
                    return Results.Ok(customers);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to retrieve customers",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status404NotFound);
                }
            })
            .WithName("GetAllCustomers")
            .WithOpenApi()
            .WithTags("Customers");
        
        // Get customer by id
        app.MapGet("/customers/{id}", async (
                Guid id,
                [FromServices] ICustomerService customerService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var customer = await customerService.GetCustomerById(id);
                    return Results.Ok(customer);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to retrieve customer",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status404NotFound);
                }
            })
            .WithName("GetCustomerById")
            .WithOpenApi()
            .WithTags("Customers");
        
        
        // Create customer
        app.MapPost("/customers", async (
                [FromBody] CreateCustomerDTO createDto,
                [FromServices] ICustomerService customerService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var createdCustomer = await customerService.AddCustomer(createDto);
                    return Results.Ok(createdCustomer);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to create customer",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("CreateCustomer")
            .WithOpenApi()
            .WithTags("Customers");
        
        // Delete customer
        app.MapDelete("/customers/{id}", async (
                Guid id,
                [FromServices] ICustomerService customerService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    await customerService.DeleteCustomer(id);
                    return Results.Ok();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Failed to create customer",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("DeleteCustomer")
            .WithOpenApi()
            .WithTags("Customers");
        
        return app;
    }
}