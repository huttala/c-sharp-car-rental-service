using System.Reflection;
using System.Text.Json.Serialization;
using CarRental.Api;
using CarRental.Api.Endpoints;
using CarRental.Application.DTOs.Booking;
using CarRental.Application.DTOs.Customer;
using CarRental.Application.DTOs.Vehicle;
using CarRental.Application.Services;
using CarRental.Application.Validators;
using CarRental.Domain.Repositories;
using CarRental.Infrastructure.DbConfiguration;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
// Configure database
builder.Services.AddDbContext<CarRentalContext>(options =>
{
    // Get connection string from configuration
    var connectionString = builder.Configuration.GetConnectionString("PostgreSQL") 
                           ?? throw new InvalidOperationException("Connection string 'PostgreSQL' not found.");
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Configure PostgreSQL-specific options
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });
    
    // Configure EF Core behavior
    if (builder.Environment.IsDevelopment())
    {
        // Enable sensitive data logging for development only
        options.EnableSensitiveDataLogging();
        
        // Better detailed errors
        options.EnableDetailedErrors();
    }
});

// Register repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

// Register services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Add FluentValidation
// builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<CreateBookingDTO>, CreateBookingDtoValidator>();
builder.Services.AddScoped<IValidator<CreateCustomerDTO>, CreateCustomerDtoValidator>();
builder.Services.AddScoped<IValidator<CreateVehicleDTO>, CreateVehicleDtoValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<CreateBookingDtoValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerDtoValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<CreateVehicleDtoValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configure health checks to include database
builder.Services.AddHealthChecks();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Car Rental API",
        Version = "v1",
        Description = "API for the Car Rental application",
        Contact = new OpenApiContact
        {
            Name = "Car Rental Team",
            Email = "Niklas@TheCarRentalCompany.com"
        }
    });
});

builder.Services.AddProblemDetails();

// Make enums readable.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Enable Swagger and Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Rental API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Car Rental API Documentation";
    options.DefaultModelsExpandDepth(2); // Control the depth of model expansion
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // Expand the operation list
});

// Map API endpoints
app.MapCustomerEndpoints();
app.MapVehicleEndpoints();
app.MapBookingEndpoints();


// Redirect to swagger UI when going to root url.
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();


// Map default endpoints
// app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Enable Swagger and Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Rental API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Car Rental API Documentation";
    options.DefaultModelsExpandDepth(2); // Control the depth of model expansion
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // Expand the operation list
});

app.UseHttpsRedirection();

// Seed the database with initial data
if (app.Environment.IsDevelopment())
{
    await DatabaseSeeder.SeedDatabase(app.Services);
}

app.Run();