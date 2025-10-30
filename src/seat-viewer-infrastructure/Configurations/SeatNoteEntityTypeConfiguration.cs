using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using seat_viewer_domain.SeatNotes;

namespace seat_viewer_infrastructure.Configurations;

/// <summary>
/// Entity configuration for SeatNote entity.
/// </summary>
public class SeatNoteEntityTypeConfiguration : IEntityTypeConfiguration<SeatNote>
{
    public void Configure(EntityTypeBuilder<SeatNote> builder)
    {
        builder.ToTable("SeatNotes");
        
        // Primary key
        builder.HasKey(n => n.Id);
        
        builder.Property(n => n.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(n => n.AircraftModel)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(n => n.SeatNumber)
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(n => n.Text)
            .HasMaxLength(500)
            .IsRequired();
        
        builder.Property(n => n.CreatedAt)
            .IsRequired();
        
        builder.Property(n => n.UpdatedAt)
            .IsRequired();
        
        // Composite index for foreign key + UpdatedAt for ordering
        builder.HasIndex(n => new { n.AircraftModel, n.SeatNumber, n.UpdatedAt });
    }
}
