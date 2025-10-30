using Microsoft.EntityFrameworkCore;
using seat_viewer_domain.SeatNotes;
using seat_viewer_domain.Seats;
using seat_viewer_infrastructure;
using seat_viewer_infrastructure.SeatNotes;

namespace seat_viewer_tests.integration;

public class SeatNoteServiceUpdateDeleteTests : IDisposable
{
    private readonly SeatViewerDbContext _context;
    private readonly ISeatNoteService _service;
    
    public SeatNoteServiceUpdateDeleteTests()
    {
        var options = new DbContextOptionsBuilder<SeatViewerDbContext>()
            .UseInMemoryDatabase(databaseName: $"SeatNoteUpdateDeleteTest_{Guid.NewGuid()}")
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
    public async Task UpdateNoteAsync_ExistingNote_UpdatesSuccessfully()
    {
        // Create a note first
        var note = await _service.CreateNoteAsync("A320", "12A", "Original text", CancellationToken.None);
        
        // Update it
        var result = await _service.UpdateNoteAsync(note.Id, "Updated text", CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal("Updated text", result.Text);
        Assert.True(result.UpdatedAt > result.CreatedAt);
    }
    
    [Fact]
    public async Task UpdateNoteAsync_NonExistentNote_ReturnsNull()
    {
        var result = await _service.UpdateNoteAsync(999, "Some text", CancellationToken.None);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateNoteAsync_TextGetsTrimmed()
    {
        var note = await _service.CreateNoteAsync("A320", "12A", "Original", CancellationToken.None);
        
        var result = await _service.UpdateNoteAsync(note.Id, "  Updated with spaces  ", CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal("Updated with spaces", result.Text);
    }
    
    [Fact]
    public async Task DeleteNoteAsync_ExistingNote_DeletesSuccessfully()
    {
        var note = await _service.CreateNoteAsync("A320", "12A", "Note to delete", CancellationToken.None);
        
        var result = await _service.DeleteNoteAsync(note.Id, CancellationToken.None);
        
        Assert.True(result);
        
        // Verify it's gone
        var retrieved = await _service.GetNoteByIdAsync(note.Id, CancellationToken.None);
        Assert.Null(retrieved);
    }
    
    [Fact]
    public async Task DeleteNoteAsync_NonExistentNote_ReturnsFalse()
    {
        var result = await _service.DeleteNoteAsync(999, CancellationToken.None);
        
        Assert.False(result);
    }
    
    [Fact]
    public async Task UpdateNoteAsync_LastWriteWins_UpdatesTimestamp()
    {
        var note = await _service.CreateNoteAsync("A320", "12A", "First version", CancellationToken.None);
        var firstUpdateTime = note.UpdatedAt;
        
        await Task.Delay(10); // Ensure time difference
        
        var updated = await _service.UpdateNoteAsync(note.Id, "Second version", CancellationToken.None);
        
        Assert.NotNull(updated);
        Assert.True(updated.UpdatedAt > firstUpdateTime);
        Assert.Equal("Second version", updated.Text);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
