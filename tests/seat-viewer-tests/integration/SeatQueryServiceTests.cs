using Microsoft.EntityFrameworkCore;
using seat_viewer_domain.Seats;
using seat_viewer_infrastructure;
using seat_viewer_infrastructure.Seats;

namespace seat_viewer_tests.integration;

public class SeatQueryServiceTests : IDisposable
{
    private readonly SeatViewerDbContext _context;
    private readonly SeatQueryService _service;
    
    public SeatQueryServiceTests()
    {
        var options = new DbContextOptionsBuilder<SeatViewerDbContext>()
            .UseInMemoryDatabase($"SeatViewerTest_{Guid.NewGuid()}")
            .Options;
        
        _context = new SeatViewerDbContext(options);
        _service = new SeatQueryService(_context);
        
        // Seed test data
        _context.Seats.AddRange(
            new Seat
            {
                AircraftModel = "A320",
                SeatNumber = "12A",
                Position = SeatPosition.Window,
                HasWindow = true,
                PowerAvailable = true,
                PowerType = "USB-C",
                HasInSeatScreen = true,
                ExperienceSummary = "Great seat",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Seat
            {
                AircraftModel = "A320",
                SeatNumber = "12B",
                Position = SeatPosition.Middle,
                HasWindow = false,
                PowerAvailable = false,
                PowerType = null,
                HasInSeatScreen = false,
                ExperienceSummary = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        
        _context.SaveChanges();
    }
    
    [Fact]
    public async Task GetSeatAsync_ExistingSeat_ReturnsSeat()
    {
        // Act
        var result = await _service.GetSeatAsync("A320", "12A");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("A320", result.AircraftModel);
        Assert.Equal("12A", result.SeatNumber);
        Assert.Equal(SeatPosition.Window, result.Position);
        Assert.True(result.HasWindow);
        Assert.True(result.PowerAvailable);
        Assert.Equal("USB-C", result.PowerType);
    }
    
    [Fact]
    public async Task GetSeatAsync_NonExistentSeat_ReturnsNull()
    {
        // Act
        var result = await _service.GetSeatAsync("B737", "99Z");
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task ListSeatsAsync_ExistingModel_ReturnsSeats()
    {
        // Act
        var result = await _service.ListSeatsAsync("A320");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public async Task ListSeatsAsync_NonExistentModel_ReturnsEmptyList()
    {
        // Act
        var result = await _service.ListSeatsAsync("B777");
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
