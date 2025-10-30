using Microsoft.EntityFrameworkCore;
using seat_viewer_domain.Seats;

namespace seat_viewer_infrastructure.Seats;

/// <summary>
/// Implementation of seat query service using Entity Framework Core.
/// </summary>
public class SeatQueryService : ISeatQueryService
{
    private readonly SeatViewerDbContext _context;
    
    public SeatQueryService(SeatViewerDbContext context)
    {
        _context = context;
    }
    
    public async Task<Seat?> GetSeatAsync(
        string aircraftModel,
        string seatNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Seats
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.AircraftModel == aircraftModel && s.SeatNumber == seatNumber,
                cancellationToken);
    }
    
    public async Task<IReadOnlyList<Seat>> ListSeatsAsync(
        string aircraftModel,
        CancellationToken cancellationToken = default)
    {
        return await _context.Seats
            .AsNoTracking()
            .Where(s => s.AircraftModel == aircraftModel)
            .OrderBy(s => s.SeatNumber)
            .ToListAsync(cancellationToken);
    }
}
