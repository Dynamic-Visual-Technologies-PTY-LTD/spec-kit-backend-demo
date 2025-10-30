using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using seat_viewer_domain.SeatNotes;
using seat_viewer_domain.Validation;
using System.Diagnostics;

namespace seat_viewer_api.Endpoints;

/// <summary>
/// Endpoints for seat note operations.
/// </summary>
public static class SeatNoteEndpoints
{
    public static void MapSeatNoteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/seats/{model}/{seat}/notes")
            .WithTags("Seat Notes");
        
        group.MapPost("", CreateNote)
            .WithName("CreateSeatNote")
            .WithSummary("Create a note for a seat")
            .WithDescription("Creates a new public note for a specific seat. Notes are limited to 500 characters.")
            .Produces<SeatNoteDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapGet("", ListNotes)
            .WithName("ListSeatNotes")
            .WithSummary("List notes for a seat")
            .WithDescription("Retrieves all public notes for a specific seat, ordered by most recent.")
            .Produces<List<SeatNoteDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        
        var noteGroup = app.MapGroup("/notes")
            .WithTags("Seat Notes");
        
        noteGroup.MapPut("/{noteId:int}", UpdateNote)
            .WithName("UpdateSeatNote")
            .WithSummary("Update a note")
            .WithDescription("Updates an existing note's text. Uses last-write-wins strategy.")
            .Produces<SeatNoteDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        
        noteGroup.MapDelete("/{noteId:int}", DeleteNote)
            .WithName("DeleteSeatNote")
            .WithSummary("Delete a note")
            .WithDescription("Deletes an existing note by ID.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
    
    private static async Task<Results<Created<SeatNoteDto>, BadRequest<string>, NotFound>> CreateNote(
        string model,
        string seat,
        CreateNoteRequest request,
        ISeatNoteService noteService,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        using var activity = Activity.Current?.Source.StartActivity("CreateSeatNote");
        activity?.SetTag("seat.model", model);
        activity?.SetTag("seat.number", seat);
        
        logger.LogInformation("Creating note for seat {Model}/{Seat}", model, seat);
        
        // Validate seat number format
        if (!SeatValidation.IsValidSeatNumber(seat))
        {
            logger.LogWarning("Invalid seat number format: {Seat}", seat);
            activity?.SetTag("validation.failed", true);
            return TypedResults.BadRequest($"Invalid seat number format: {seat}");
        }
        
        // Validate note text
        if (!NoteValidation.IsValidNoteText(request.Text))
        {
            logger.LogWarning("Invalid note text: length={Length}", request.Text?.Length ?? 0);
            activity?.SetTag("validation.failed", true);
            return TypedResults.BadRequest("Note text must be between 1 and 500 characters");
        }
        
        try
        {
            var note = await noteService.CreateNoteAsync(model, seat, request.Text, cancellationToken);
            
            activity?.SetTag("note.id", note.Id);
            logger.LogInformation("Note created successfully: {NoteId} for seat {Model}/{Seat}", note.Id, model, seat);
            
            var dto = new SeatNoteDto
            {
                Id = note.Id,
                AircraftModel = note.AircraftModel,
                SeatNumber = note.SeatNumber,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            };
            
            return TypedResults.Created($"/seats/{model}/{seat}/notes/{note.Id}", dto);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Seat not found: {Model}/{Seat}", model, seat);
            activity?.SetTag("seat.found", false);
            return TypedResults.NotFound();
        }
    }
    
    private static async Task<Results<Ok<List<SeatNoteDto>>, BadRequest<string>>> ListNotes(
        string model,
        string seat,
        ISeatNoteService noteService,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        using var activity = Activity.Current?.Source.StartActivity("ListSeatNotes");
        activity?.SetTag("seat.model", model);
        activity?.SetTag("seat.number", seat);
        
        logger.LogInformation("Listing notes for seat {Model}/{Seat}", model, seat);
        
        // Validate seat number format
        if (!SeatValidation.IsValidSeatNumber(seat))
        {
            logger.LogWarning("Invalid seat number format: {Seat}", seat);
            activity?.SetTag("validation.failed", true);
            return TypedResults.BadRequest($"Invalid seat number format: {seat}");
        }
        
        var notes = await noteService.ListNotesAsync(model, seat, cancellationToken);
        
        activity?.SetTag("notes.count", notes.Count);
        logger.LogInformation("Retrieved {Count} notes for seat {Model}/{Seat}", notes.Count, model, seat);
        
        var dtos = notes.Select(n => new SeatNoteDto
        {
            Id = n.Id,
            AircraftModel = n.AircraftModel,
            SeatNumber = n.SeatNumber,
            Text = n.Text,
            CreatedAt = n.CreatedAt,
            UpdatedAt = n.UpdatedAt
        }).ToList();
        
        return TypedResults.Ok(dtos);
    }
    
    private static async Task<Results<Ok<SeatNoteDto>, BadRequest<string>, NotFound>> UpdateNote(
        int noteId,
        UpdateNoteRequest request,
        ISeatNoteService noteService,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        using var activity = Activity.Current?.Source.StartActivity("UpdateSeatNote");
        activity?.SetTag("note.id", noteId);
        
        logger.LogInformation("Updating note {NoteId}", noteId);
        
        // Validate note text
        if (!NoteValidation.IsValidNoteText(request.Text))
        {
            logger.LogWarning("Invalid note text: length={Length}", request.Text?.Length ?? 0);
            activity?.SetTag("validation.failed", true);
            return TypedResults.BadRequest("Note text must be between 1 and 500 characters");
        }
        
        var note = await noteService.UpdateNoteAsync(noteId, request.Text, cancellationToken);
        
        if (note is null)
        {
            logger.LogWarning("Note not found: {NoteId}", noteId);
            activity?.SetTag("note.found", false);
            return TypedResults.NotFound();
        }
        
        activity?.SetTag("note.updated", true);
        logger.LogInformation("Note updated successfully: {NoteId}", noteId);
        
        var dto = new SeatNoteDto
        {
            Id = note.Id,
            AircraftModel = note.AircraftModel,
            SeatNumber = note.SeatNumber,
            Text = note.Text,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };
        
        return TypedResults.Ok(dto);
    }
    
    private static async Task<Results<NoContent, NotFound>> DeleteNote(
        int noteId,
        ISeatNoteService noteService,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        using var activity = Activity.Current?.Source.StartActivity("DeleteSeatNote");
        activity?.SetTag("note.id", noteId);
        
        logger.LogInformation("Deleting note {NoteId}", noteId);
        
        var deleted = await noteService.DeleteNoteAsync(noteId, cancellationToken);
        
        if (!deleted)
        {
            logger.LogWarning("Note not found: {NoteId}", noteId);
            activity?.SetTag("note.found", false);
            return TypedResults.NotFound();
        }
        
        activity?.SetTag("note.deleted", true);
        logger.LogInformation("Note deleted successfully: {NoteId}", noteId);
        
        return TypedResults.NoContent();
    }
}

public record CreateNoteRequest
{
    public required string Text { get; init; }
}

public record UpdateNoteRequest
{
    public required string Text { get; init; }
}

public record SeatNoteDto
{
    public required int Id { get; init; }
    public required string AircraftModel { get; init; }
    public required string SeatNumber { get; init; }
    public required string Text { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
