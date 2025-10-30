using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using seat_viewer_domain.Seats;

namespace seat_viewer_infrastructure.Configurations;

/// <summary>
/// Entity configuration for Seat entity.
/// </summary>
public class SeatEntityTypeConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seats");
        
        // Composite primary key
        builder.HasKey(s => new { s.AircraftModel, s.SeatNumber });
        
        builder.Property(s => s.AircraftModel)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(s => s.SeatNumber)
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(s => s.Position)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(s => s.HasWindow)
            .IsRequired();
        
        builder.Property(s => s.PowerAvailable)
            .IsRequired();
        
        builder.Property(s => s.PowerType)
            .HasMaxLength(20);
        
        builder.Property(s => s.HasInSeatScreen)
            .IsRequired();
        
        builder.Property(s => s.ExperienceSummary)
            .HasMaxLength(500);
        
        builder.Property(s => s.CreatedAt)
            .IsRequired();
        
        builder.Property(s => s.UpdatedAt)
            .IsRequired();
        
        // Index on Position for filtering
        builder.HasIndex(s => s.Position);
        
        // Relationship to notes
        builder.HasMany(s => s.Notes)
            .WithOne(n => n.Seat)
            .HasForeignKey(n => new { n.AircraftModel, n.SeatNumber })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
