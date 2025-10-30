using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using seat_viewer_domain.Seats;
using seat_viewer_domain.SeatNotes;
using seat_viewer_infrastructure.Seats;
using seat_viewer_infrastructure.SeatNotes;

namespace seat_viewer_infrastructure.Extensions;

/// <summary>
/// Extension methods for registering infrastructure services.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<SeatViewerDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SeatViewerDb"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()));
        
        // Register services
        services.AddScoped<ISeatQueryService, SeatQueryService>();
        services.AddScoped<ISeatNoteService, SeatNoteService>();
        
        return services;
    }
}
