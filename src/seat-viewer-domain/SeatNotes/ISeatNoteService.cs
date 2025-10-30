namespace seat_viewer_domain.SeatNotes;

/// <summary>
/// Service for managing seat notes. Provides CRUD operations for public seat notes with last-write-wins concurrency.
/// </summary>
public interface ISeatNoteService
{
    /// <summary>
    /// Lists all notes for a specific seat, ordered by most recent first.
    /// </summary>
    /// <param name="aircraftModel">The aircraft model identifier.</param>
    /// <param name="seatNumber">The seat number.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Read-only collection of notes, ordered by UpdatedAt descending. Empty if none found.</returns>
    Task<IReadOnlyList<SeatNote>> ListNotesAsync(string aircraftModel, string seatNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new note for a seat. Text will be sanitized (trimmed).
    /// </summary>
    /// <param name="aircraftModel">The aircraft model identifier.</param>
    /// <param name="seatNumber">The seat number.</param>
    /// <param name="text">The note text (max 500 characters).</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The created note with assigned ID and timestamps.</returns>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">Thrown if the seat does not exist.</exception>
    Task<SeatNote> CreateNoteAsync(string aircraftModel, string seatNumber, string text, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing note's text. Uses last-write-wins strategy. Text will be sanitized (trimmed).
    /// </summary>
    /// <param name="noteId">The ID of the note to update.</param>
    /// <param name="text">The new note text (max 500 characters).</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The updated note with new timestamp, or null if not found.</returns>
    Task<SeatNote?> UpdateNoteAsync(int noteId, string text, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a note by ID.
    /// </summary>
    /// <param name="noteId">The ID of the note to delete.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if the note was deleted, false if not found.</returns>
    Task<bool> DeleteNoteAsync(int noteId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single note by ID.
    /// </summary>
    /// <param name="noteId">The ID of the note to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The note if found, otherwise null.</returns>
    Task<SeatNote?> GetNoteByIdAsync(int noteId, CancellationToken cancellationToken = default);
}
