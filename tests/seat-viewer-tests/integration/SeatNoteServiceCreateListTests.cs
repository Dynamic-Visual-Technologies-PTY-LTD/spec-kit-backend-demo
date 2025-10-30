using Microsoft.EntityFrameworkCore;
using seat_viewer_domain.SeatNotes;
using seat_viewer_domain.Seats;
using seat_viewer_infrastructure;
using seat_viewer_infrastructure.SeatNotes;

namespace seat_viewer_tests.integration;

public class SeatNoteServiceCreateListTests : IDisposable
{
    private readonly SeatViewerDbContext _context;
    private readonly ISeatNoteService _service;
    
    public SeatNoteServiceCreateListTests()
    {
        var options = new DbContextOptionsBuilder<SeatViewerDbContext>()
            .UseInMemoryDatabase(databaseName: $"SeatNoteServiceTest_{Guid.NewGuid()}")
            .Options;
        
        _context = new SeatViewerDbContext(options);
        _service = new SeatNoteService(_context);
        
        // Seed test data
        var seat = new Seat
        {
            AircraftModel = "A320",
            SeatNumber = "12A",
            Position = SeatPosition.Window,
            HasWindow = true,
            PowerAvailable = true,
            PowerType = "USB-C",
            HasInSeatScreen = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.Seats.Add(seat);
        _context.SaveChanges();
    }
    
    [Fact]
    public async Task CreateNoteAsync_ValidNote_CreatesSuccessfully()
    {
        var result = await _service.CreateNoteAsync(
            "A320",
            "12A",
            "Great seat with extra legroom!",
            CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Great seat with extra legroom!", result.Text);
        Assert.Equal("A320", result.AircraftModel);
        Assert.Equal("12A", result.SeatNumber);
    }
    
    [Fact]
    public async Task CreateNoteAsync_TextGetsTrimmed()
    {
        var result = await _service.CreateNoteAsync(
            "A320",
            "12A",
            "  Note with spaces  ",
            CancellationToken.None);
        
        Assert.Equal("Note with spaces", result.Text);
    }
    
    [Fact]
    public async Task ListNotesAsync_ReturnsAllNotesForSeat()
    {
        // Create multiple notes
        await _service.CreateNoteAsync("A320", "12A", "First note", CancellationToken.None);
        await _service.CreateNoteAsync("A320", "12A", "Second note", CancellationToken.None);
        
        var notes = await _service.ListNotesAsync("A320", "12A", CancellationToken.None);
        
        Assert.Equal(2, notes.Count);
        Assert.Contains(notes, n => n.Text == "First note");
        Assert.Contains(notes, n => n.Text == "Second note");
    }
    
    [Fact]
    public async Task ListNotesAsync_NoNotes_ReturnsEmptyList()
    {
        var notes = await _service.ListNotesAsync("A320", "12A", CancellationToken.None);
        
        Assert.Empty(notes);
    }
    
    [Fact]
    public async Task ListNotesAsync_DifferentSeat_ReturnsEmpty()
    {
        await _service.CreateNoteAsync("A320", "12A", "Note for 12A", CancellationToken.None);
        
        var notes = await _service.ListNotesAsync("A320", "12B", CancellationToken.None);
        
        Assert.Empty(notes);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
