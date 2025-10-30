using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using seat_viewer_domain.Seats;
using seat_viewer_domain.SeatNotes;

namespace seat_viewer_infrastructure.Seed;

/// <summary>
/// Seeds initial seat data and sample notes for development and testing.
/// </summary>
public static class SeatDataSeeder
{
    public static async Task SeedAsync(SeatViewerDbContext context, ILogger logger)
    {
        logger.LogInformation("Checking for seed data...");
        
        if (await context.Seats.AnyAsync())
        {
            logger.LogInformation("Seed data already exists. Skipping.");
            return;
        }
        
        logger.LogInformation("Seeding initial seat data...");
        
        var seats = new List<Seat>
        {
            new()
            {
                AircraftModel = "A320",
                SeatNumber = "12A",
                Position = SeatPosition.Window,
                HasWindow = true,
                PowerAvailable = true,
                PowerType = "USB-C",
                HasInSeatScreen = true,
                ExperienceSummary = "Quiet row, good legroom",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                AircraftModel = "A320",
                SeatNumber = "12B",
                Position = SeatPosition.Middle,
                HasWindow = false,
                PowerAvailable = true,
                PowerType = "USB",
                HasInSeatScreen = true,
                ExperienceSummary = "Standard middle seat",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                AircraftModel = "A320",
                SeatNumber = "12C",
                Position = SeatPosition.Aisle,
                HasWindow = false,
                PowerAvailable = true,
                PowerType = "USB-C",
                HasInSeatScreen = true,
                ExperienceSummary = "Easy access to aisle",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                AircraftModel = "B737",
                SeatNumber = "15F",
                Position = SeatPosition.Window,
                HasWindow = false,
                PowerAvailable = false,
                PowerType = null,
                HasInSeatScreen = false,
                ExperienceSummary = "Exit row, extra legroom but no window",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        
        context.Seats.AddRange(seats);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Seed data added: {Count} seats", seats.Count);

        // Add sample notes for demonstration
        logger.LogInformation("Seeding sample notes...");
        
        var baseTime = DateTime.UtcNow.AddDays(-7);
        var notes = new List<SeatNote>
        {
            new()
            {
                AircraftModel = "A320",
                SeatNumber = "12A",
                Text = "Great legroom and easy access to overhead bins. The window view is excellent for photography!",
                CreatedAt = baseTime,
                UpdatedAt = baseTime
            },
            new()
            {
                AircraftModel = "A320",
                SeatNumber = "12A",
                Text = "Power outlet works perfectly for laptop charging during long flights.",
                CreatedAt = baseTime.AddDays(2),
                UpdatedAt = baseTime.AddDays(2)
            },
            new()
            {
                AircraftModel = "A320",
                SeatNumber = "12B",
                Text = "Too cramped as a middle seat. No power outlet is disappointing on long-haul flights.",
                CreatedAt = baseTime.AddDays(1),
                UpdatedAt = baseTime.AddDays(1)
            },
            new()
            {
                AircraftModel = "A320",
                SeatNumber = "12C",
                Text = "Convenient aisle access for frequent bathroom trips. USB-C power is very helpful.",
                CreatedAt = baseTime.AddDays(3),
                UpdatedAt = baseTime.AddDays(3)
            },
            new()
            {
                AircraftModel = "B737",
                SeatNumber = "15F",
                Text = "Exit row means extra legroom, but the lack of window is a major downside. No recline either.",
                CreatedAt = baseTime.AddDays(4),
                UpdatedAt = baseTime.AddDays(4)
            },
            new()
            {
                AircraftModel = "B737",
                SeatNumber = "15F",
                Text = "Perfect for tall passengers needing leg space. Just don't expect scenery or under-seat storage.",
                CreatedAt = baseTime.AddDays(5),
                UpdatedAt = baseTime.AddDays(5)
            }
        };

        context.SeatNotes.AddRange(notes);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Seed data added: {Count} notes", notes.Count);
    }
}
