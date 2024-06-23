using System.Net;

namespace PokemonLookup.IntegrationTests;

/// <summary>
/// Tests important Use-Cases of the application.
/// </summary>
/// <param name="factory">Used to set up the application dependencies</param>
public class PokemonDetailsPageTest(TestingWebAppFactory factory)
    : IClassFixture<TestingWebAppFactory>
{
    /// <summary>
    /// Test a request for an existing Pokémon.
    /// The Pokémon should be found and displayed as an HTML page.
    /// </summary>
    [Fact]
    public async Task GetNewValidPokemon()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/pokemon?name=ditto");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }

    /// <summary>
    /// Test the caching capabilities of the application.
    /// The Pokémon should be found and displayed even after repeated requests.
    /// </summary>
    [Fact]
    public async Task GetRepeatedValidPokemon()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        HttpResponseMessage response = null!;
        for (var i = 0; i < 5; i++) // Simulate multiple lookups for the same Pokémon
        {
            response = await client.GetAsync("/pokemon?name=pikachu");
        }

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }

    /// <summary>
    /// Test a lookup of an unknown Pokémon.
    /// A 404 response in combination with an error page is expected.
    /// </summary>
    [Fact]
    public async Task GetUnknownPokemon()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/pokemon?name=doesNotExist");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }
}
