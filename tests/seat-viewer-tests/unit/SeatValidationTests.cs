using seat_viewer_domain.Validation;

namespace seat_viewer_tests.unit;

public class SeatValidationTests
{
    [Fact]
    public void IsValidSeatNumber_ValidSeatNumber_ReturnsTrue()
    {
        // Arrange & Act
        var result = SeatValidation.IsValidSeatNumber("12A");
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("1A")]
    [InlineData("99F")]
    [InlineData("123B")]
    public void IsValidSeatNumber_ValidPatterns_ReturnsTrue(string seatNumber)
    {
        // Act
        var result = SeatValidation.IsValidSeatNumber(seatNumber);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("12")]
    [InlineData("A12")]
    [InlineData("12G")]
    [InlineData("12AA")]
    [InlineData("AB")]
    public void IsValidSeatNumber_InvalidPatterns_ReturnsFalse(string? seatNumber)
    {
        // Act
        var result = SeatValidation.IsValidSeatNumber(seatNumber!);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void ValidatePowerConfiguration_PowerAvailableWithType_ReturnsTrue()
    {
        // Act
        var result = SeatValidation.ValidatePowerConfiguration(true, "USB-C");
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void ValidatePowerConfiguration_PowerAvailableWithoutType_ReturnsFalse()
    {
        // Act
        var result = SeatValidation.ValidatePowerConfiguration(true, null);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void ValidatePowerConfiguration_NoPowerNoType_ReturnsTrue()
    {
        // Act
        var result = SeatValidation.ValidatePowerConfiguration(false, null);
        
        // Assert
        Assert.True(result);
    }
}
