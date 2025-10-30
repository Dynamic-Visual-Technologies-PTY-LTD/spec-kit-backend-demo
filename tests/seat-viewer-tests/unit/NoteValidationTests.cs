using seat_viewer_domain.Validation;

namespace seat_viewer_tests.unit;

public class NoteValidationTests
{
    [Theory]
    [InlineData("Valid note text")]
    [InlineData("A")]
    [InlineData("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua Ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur Excepteur sint occaecat cupidatat non proident sunt in culpa qui officia deserunt mollit anim id est laborum curabitur preti")]
    public void IsValidNoteText_ValidText_ReturnsTrue(string text)
    {
        var result = NoteValidation.IsValidNoteText(text);
        
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValidNoteText_EmptyOrWhitespace_ReturnsFalse(string? text)
    {
        var result = NoteValidation.IsValidNoteText(text);
        
        Assert.False(result);
    }
    
    [Fact]
    public void IsValidNoteText_TextExceeds500Characters_ReturnsFalse()
    {
        var text = new string('x', 501);
        
        var result = NoteValidation.IsValidNoteText(text);
        
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("  test  ", "test")]
    [InlineData("note\n\r", "note")]
    [InlineData("\t\tindented\t\t", "indented")]
    public void SanitizeNoteText_TrimsWhitespace(string input, string expected)
    {
        var result = NoteValidation.SanitizeNoteText(input);
        
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void SanitizeNoteText_NullInput_ReturnsEmptyString()
    {
        var result = NoteValidation.SanitizeNoteText(null);
        
        Assert.Equal(string.Empty, result);
    }
}
