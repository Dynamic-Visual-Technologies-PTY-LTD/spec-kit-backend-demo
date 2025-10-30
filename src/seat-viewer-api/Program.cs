using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using seat_viewer_infrastructure.Extensions;
using Scalar.AspNetCore;
using seat_viewer_infrastructure;
using Microsoft.EntityFrameworkCore;
using seat_viewer_infrastructure.Seed;
using seat_viewer_api.Endpoints;
using seat_viewer_api.Middleware;

// Bootstrap logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Seat Viewer API");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog with correlation ID enrichment
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: builder.Environment.IsDevelopment()
                ? "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
                : "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
            restrictedToMinimumLevel: builder.Environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information));

    // Add OpenTelemetry with correlation tracking
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing => tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("seat-viewer-api", serviceVersion: "0.1.0"))
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, httpRequest) =>
                {
                    activity.SetTag("http.request.correlation_id", httpRequest.HttpContext.TraceIdentifier);
                };
            })
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddConsoleExporter());

    // Add services to the container
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();

    // Add infrastructure (DbContext, services)
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Add error handling middleware first
    app.UseMiddleware<ErrorHandlingMiddleware>();

    // Apply migrations and seed data in development
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SeatViewerDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Applying database migrations...");
        await dbContext.Database.MigrateAsync();
        
        await SeatDataSeeder.SeedAsync(dbContext, logger);
    }

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Seat Viewer API";
        });
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "unknown");
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        };
    });

    app.UseHttpsRedirection();

    // Map endpoints
    app.MapSeatEndpoints();
    app.MapSeatNoteEndpoints();
    
    // Sample endpoint (will be replaced)
    app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
        .WithName("Health")
        .WithTags("Diagnostics");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }

