using Microsoft.EntityFrameworkCore;
using seat_viewer_domain.Seats;
using seat_viewer_domain.SeatNotes;

namespace seat_viewer_infrastructure;

/// <summary>
/// Entity Framework Core database context for Seat Viewer application.
/// </summary>
public class SeatViewerDbContext : DbContext
{
    public SeatViewerDbContext(DbContextOptions<SeatViewerDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Seat> Seats => Set<Seat>();
    
    public DbSet<SeatNote> SeatNotes => Set<SeatNote>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeatViewerDbContext).Assembly);
    }
}
