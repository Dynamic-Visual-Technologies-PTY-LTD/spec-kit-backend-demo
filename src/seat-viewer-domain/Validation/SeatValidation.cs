using System.Text.RegularExpressions;

namespace seat_viewer_domain.Validation;

/// <summary>
/// Validation helpers for seat-related data.
/// </summary>
public static class SeatValidation
{
    private static readonly Regex SeatNumberPattern = new(@"^[0-9]+[A-F]$", RegexOptions.Compiled);
    
    /// <summary>
    /// Validates seat number format (row digits + seat letter A-F).
    /// </summary>
    public static bool IsValidSeatNumber(string seatNumber)
    {
        if (string.IsNullOrWhiteSpace(seatNumber))
        {
            return false;
        }
            
        return SeatNumberPattern.IsMatch(seatNumber);
    }
    
    /// <summary>
    /// Validates power type is required when power is available.
    /// </summary>
    public static bool ValidatePowerConfiguration(bool powerAvailable, string? powerType)
    {
        if (powerAvailable && string.IsNullOrWhiteSpace(powerType))
        {
            return false;
        }
            
        return true;
    }
}
