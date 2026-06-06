using CruiseSearch.Api.Models;
using CruiseSearch.Api.Providers;
using CruiseSearch.Api.Services;
using CruiseSearch.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddScoped<OceanLuxProvider>();
builder.Services.AddScoped<IslandHopProvider>();
builder.Services.AddScoped<RiverVoyageProvider>();
builder.Services.AddScoped<CruiseAggregatorService>();
builder.Services.AddScoped<BookingValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// ============ API ENDPOINTS ============

// GET /cruises/search
app.MapGet("/cruises/search", async (
    string? departurePort,
    string? date,
    string? cabinClass,
    int passengers = 2,
    CruiseAggregatorService aggregator) =>
{
    // Validation
    if (string.IsNullOrWhiteSpace(departurePort) || string.IsNullOrWhiteSpace(date))
    {
        return Results.BadRequest(new ErrorResponse
        {
            Error = "Validation Error",
            Message = "departurePort and date are required."
        });
    }

    if (passengers < 1 || passengers > 4)
    {
        return Results.BadRequest(new ErrorResponse
        {
            Error = "Validation Error",
            Message = "passengers must be between 1 and 4."
        });
    }

    if (!DateTime.TryParse(date, out var sailingDate))
    {
        return Results.BadRequest(new ErrorResponse
        {
            Error = "Validation Error",
            Message = "date must be a valid ISO 8601 date."
        });
    }

    CabinClass? cabinClassEnum = null;
    if (!string.IsNullOrWhiteSpace(cabinClass))
    {
        if (!Enum.TryParse<CabinClass>(cabinClass, out var parsed))
        {
            return Results.BadRequest(new ErrorResponse
            {
                Error = "Validation Error",
                Message = $"cabinClass must be one of: Interior, OceanView, Balcony, Suite."
            });
        }
        cabinClassEnum = parsed;
    }

    var result = await aggregator.SearchAsync(departurePort, sailingDate, cabinClassEnum, passengers);
    return Results.Ok(result);
})
.WithName("SearchCruises")
.WithOpenApi();

// POST /cruises/book
app.MapPost("/cruises/book", async (
    BookingRequest request,
    CruiseAggregatorService aggregator,
    BookingValidator validator) =>
{
    var (isValid, errorMessage) = validator.ValidateBooking(request);
    if (!isValid)
    {
        return Results.UnprocessableEntity(new ErrorResponse
        {
            Error = "Document validation failed",
            Message = errorMessage ?? "Invalid document."
        });
    }

    try
    {
        var confirmation = await aggregator.BookAsync(request);
        return Results.Ok(confirmation);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new ErrorResponse
        {
            Error = "Booking Error",
            Message = ex.Message
        });
    }
})
.WithName("BookCruise")
.WithOpenApi();

// GET /cruises/booking/{reference}
app.MapGet("/cruises/booking/{reference}", async (
    string reference,
    CruiseAggregatorService aggregator) =>
{
    var booking = await aggregator.GetBookingStatusAsync(reference);
    if (booking == null)
    {
        return Results.NotFound(new ErrorResponse
        {
            Error = "Not Found",
            Message = $"Booking reference {reference} not found."
        });
    }

    return Results.Ok(booking);
})
.WithName("GetBookingStatus")
.WithOpenApi();

app.Run();
