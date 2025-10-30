namespace seat_viewer_domain.Seats;

/// <summary>
/// Service for querying seat data. Provides read-only access to seat information.
/// </summary>
public interface ISeatQueryService
{
    /// <summary>
    /// Retrieves a seat by aircraft model and seat number.
    /// </summary>
    /// <param name="aircraftModel">The aircraft model identifier (e.g., "A320", "B737").</param>
    /// <param name="seatNumber">The seat number in format: digits + letter A-F (e.g., "12A", "15F").</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The seat if found, otherwise null.</returns>
    Task<Seat?> GetSeatAsync(string aircraftModel, string seatNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists all seats for a given aircraft model.
    /// </summary>
    /// <param name="aircraftModel">The aircraft model identifier (e.g., "A320", "B737").</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Read-only collection of seats for the specified aircraft model. Empty if none found.</returns>
    Task<IReadOnlyList<Seat>> ListSeatsAsync(string aircraftModel, CancellationToken cancellationToken = default);
}
