namespace seat_viewer_domain.Seats;

/// <summary>
/// Represents a seat on an aircraft with its attributes.
/// </summary>
public class Seat
{
    public required string AircraftModel { get; set; }
    
    public required string SeatNumber { get; set; }
    
    public required SeatPosition Position { get; set; }
    
    public required bool HasWindow { get; set; }
    
    public required bool PowerAvailable { get; set; }
    
    public string? PowerType { get; set; }
    
    public required bool HasInSeatScreen { get; set; }
    
    public string? ExperienceSummary { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property
    public ICollection<SeatNotes.SeatNote> Notes { get; set; } = new List<SeatNotes.SeatNote>();
    
    /// <summary>
    /// Validates if this is a window seat with an actual window.
    /// </summary>
    public bool WindowConfirmed => HasWindow && Position == SeatPosition.Window;
}

public enum SeatPosition
{
    Aisle,
    Middle,
    Window
}
