namespace seat_viewer_domain.Validation;

/// <summary>
/// Validation helpers for seat note data.
/// </summary>
public static class NoteValidation
{
    public const int MaxNoteLength = 500;
    
    /// <summary>
    /// Validates note text length (max 500 characters).
    /// </summary>
    public static bool IsValidNoteText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }
            
        return text.Length <= MaxNoteLength;
    }
    
    /// <summary>
    /// Sanitizes note text by trimming whitespace.
    /// </summary>
    public static string SanitizeNoteText(string? text)
    {
        return text?.Trim() ?? string.Empty;
    }
}
