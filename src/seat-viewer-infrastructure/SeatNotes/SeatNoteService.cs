using Microsoft.EntityFrameworkCore;
using seat_viewer_domain.SeatNotes;
using seat_viewer_domain.Validation;

namespace seat_viewer_infrastructure.SeatNotes;

/// <summary>
/// Implementation of seat note service using Entity Framework Core.
/// </summary>
public class SeatNoteService : ISeatNoteService
{
    private readonly SeatViewerDbContext _context;
    
    public SeatNoteService(SeatViewerDbContext context)
    {
        _context = context;
    }
    
    public async Task<IReadOnlyList<SeatNote>> ListNotesAsync(
        string aircraftModel,
        string seatNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.SeatNotes
            .AsNoTracking()
            .Where(n => n.AircraftModel == aircraftModel && n.SeatNumber == seatNumber)
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<SeatNote> CreateNoteAsync(
        string aircraftModel,
        string seatNumber,
        string text,
        CancellationToken cancellationToken = default)
    {
        var sanitizedText = NoteValidation.SanitizeNoteText(text);
        
        var note = new SeatNote
        {
            AircraftModel = aircraftModel,
            SeatNumber = seatNumber,
            Text = sanitizedText,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.SeatNotes.Add(note);
        await _context.SaveChangesAsync(cancellationToken);
        
        return note;
    }
    
    public async Task<SeatNote?> UpdateNoteAsync(
        int noteId,
        string text,
        CancellationToken cancellationToken = default)
    {
        var note = await _context.SeatNotes.FindAsync(new object[] { noteId }, cancellationToken);
        
        if (note is null)
        {
            return null;
        }
            
        var sanitizedText = NoteValidation.SanitizeNoteText(text);
        note.Text = sanitizedText;
        note.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return note;
    }
    
    public async Task<bool> DeleteNoteAsync(
        int noteId,
        CancellationToken cancellationToken = default)
    {
        var note = await _context.SeatNotes.FindAsync(new object[] { noteId }, cancellationToken);
        
        if (note is null)
        {
            return false;
        }
            
        _context.SeatNotes.Remove(note);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
    
    public async Task<SeatNote?> GetNoteByIdAsync(
        int noteId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SeatNotes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == noteId, cancellationToken);
    }
}
