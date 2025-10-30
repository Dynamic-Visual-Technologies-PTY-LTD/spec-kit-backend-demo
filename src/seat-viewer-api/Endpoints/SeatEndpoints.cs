using Microsoft.AspNetCore.Http.HttpResults;
using seat_viewer_domain.Seats;
using seat_viewer_domain.Validation;
using System.Diagnostics;

namespace seat_viewer_api.Endpoints;

/// <summary>
/// Endpoints for seat attribute operations.
/// </summary>
public static class SeatEndpoints
{
    public static void MapSeatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/seats")
            .WithTags("Seats");
        
        group.MapGet("/{model}/{seat}", GetSeatAttributes)
            .WithName("GetSeatAttributes")
            .WithSummary("Retrieve seat attributes")
            .WithDescription("Gets detailed attributes for a specific seat including position, power, screen, and experience summary.")
            .Produces<SeatAttributesDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }
    
    private static async Task<Results<Ok<SeatAttributesDto>, BadRequest<string>, NotFound>> GetSeatAttributes(
        string model,
        string seat,
        ISeatQueryService seatQueryService,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        using var activity = Activity.Current?.Source.StartActivity("GetSeatAttributes");
        activity?.SetTag("seat.model", model);
        activity?.SetTag("seat.number", seat);
        
        logger.LogInformation("Retrieving seat attributes for {Model}/{Seat}", model, seat);
        
        // Validate seat number format
        if (!SeatValidation.IsValidSeatNumber(seat))
        {
            logger.LogWarning("Invalid seat number format: {Seat}", seat);
            activity?.SetTag("validation.failed", true);
            return TypedResults.BadRequest($"Invalid seat number format: {seat}. Expected pattern: row digits + seat letter (A-F).");
        }
        
        var result = await seatQueryService.GetSeatAsync(model, seat, cancellationToken);
        
        if (result is null)
        {
            logger.LogInformation("Seat not found: {Model}/{Seat}", model, seat);
            activity?.SetTag("seat.found", false);
            return TypedResults.NotFound();
        }
        
        activity?.SetTag("seat.found", true);
        logger.LogInformation("Seat retrieved successfully: {Model}/{Seat}", model, seat);
        
        var dto = new SeatAttributesDto
        {
            AircraftModel = result.AircraftModel,
            SeatNumber = result.SeatNumber,
            Position = result.Position.ToString(),
            HasWindow = result.HasWindow,
            PowerAvailable = result.PowerAvailable,
            PowerType = result.PowerType,
            HasInSeatScreen = result.HasInSeatScreen,
            ExperienceSummary = result.ExperienceSummary
        };
        
        return TypedResults.Ok(dto);
    }
}

public record SeatAttributesDto
{
    public required string AircraftModel { get; init; }
    public required string SeatNumber { get; init; }
    public required string Position { get; init; }
    public required bool HasWindow { get; init; }
    public required bool PowerAvailable { get; init; }
    public string? PowerType { get; init; }
    public required bool HasInSeatScreen { get; init; }
    public string? ExperienceSummary { get; init; }
}
