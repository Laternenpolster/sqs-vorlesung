using System.Net;

namespace PokemonLookup.IntegrationTests;

public class PokemonDetailsPageTest(TestingWebAppFactory factory)
    : IClassFixture<TestingWebAppFactory>
{
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

    [Fact]
    public async Task GetRepeatedValidPokemon()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        HttpResponseMessage response = null!;
        for (var i = 0; i < 5; i++)
        {
            response = await client.GetAsync("/pokemon?name=pikachu");
        }

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }

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
