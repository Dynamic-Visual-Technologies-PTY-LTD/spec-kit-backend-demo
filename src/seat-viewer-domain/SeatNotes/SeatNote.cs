namespace seat_viewer_domain.SeatNotes;

/// <summary>
/// Represents a public note attached to a specific seat.
/// </summary>
public class SeatNote
{
    public int Id { get; set; }
    
    public required string AircraftModel { get; set; }
    
    public required string SeatNumber { get; set; }
    
    public required string Text { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property
    public Seats.Seat? Seat { get; set; }
}
