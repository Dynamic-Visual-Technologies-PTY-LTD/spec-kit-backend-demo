using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace seat_viewer_tests.contract;

/// <summary>
/// Validates that all expected API endpoints are exposed in the OpenAPI document.
/// Ensures Scalar UI and OpenAPI generation are properly configured.
/// </summary>
public sealed class OpenApiValidationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OpenApiValidationTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task OpenApiDocument_ShouldBeAccessible()
    {
        // Act
        var response = await _client.GetAsync("/openapi/v1.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task OpenApiDocument_ShouldContainPaths()
    {
        // Arrange
        var response = await _client.GetAsync("/openapi/v1.json");
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        // Assert - Check paths section exists and has content
        Assert.True(doc.RootElement.TryGetProperty("paths", out var paths), 
            "Paths section not found in OpenAPI document");
        
        var pathCount = 0;
        foreach (var path in paths.EnumerateObject())
        {
            pathCount++;
        }
        
        Assert.True(pathCount >= 4, 
            $"Expected at least 4 paths (seats + notes endpoints), found {pathCount}");
    }

    [Fact]
    public async Task OpenApiDocument_ShouldIncludeResponseSchemas()
    {
        // Arrange
        var response = await _client.GetAsync("/openapi/v1.json");
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        // Assert - Check components/schemas section exists
        Assert.True(doc.RootElement.TryGetProperty("components", out var components), 
            "Components section not found in OpenAPI document");
        
        Assert.True(components.TryGetProperty("schemas", out var schemas), 
            "Schemas section not found in components");

        // Verify schemas are documented (any DTOs)
        var schemaCount = 0;
        foreach (var schema in schemas.EnumerateObject())
        {
            schemaCount++;
        }

        Assert.True(schemaCount > 0, "No schemas found in OpenAPI document");
    }

    [Fact]
    public async Task ScalarUi_ShouldBeAccessible()
    {
        // Act
        var response = await _client.GetAsync("/scalar/v1");

        // Assert - Scalar UI should return HTML page
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var contentType = response.Content.Headers.ContentType?.MediaType;
        Assert.True(contentType == "text/html" || contentType == "application/html+xml", 
            $"Expected HTML content type, got {contentType}");
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}
