using Moq;
using PokemonLookup.Core.Services;
using PokemonLookup.Infrastructure.ExternalLookup;

namespace PokemonLookup.UnitTests.Services;

/// <summary>
/// Test the service used to search the Pokédex.
/// </summary>
public class PokemonApiRequesterTest
{
    private const string PokemonName = "abcdefg";

    /// <summary>
    /// Simulate a valid request.
    /// The request should be successful and the result should match the query.
    /// </summary>
    [Fact]
    public async Task TestValidRequest()
    {
        // Arrange
        var mockApi = new Mock<IApiRequester>();
        mockApi
            .Setup(service => service.GetRequest<PokedexResultDto>(It.IsAny<string>()))
            .ReturnsAsync(GetTestPokedexResult());

        var apiRequester = new PokemonApiRequester(mockApi.Object);

        // Act
        var result = await apiRequester.SearchByName(PokemonName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(GetTestPokedexResult().Name, result.Name);
    }

    /// <summary>
    /// A result from the API for a known Pokémon.
    /// </summary>
    private static PokedexResultDto GetTestPokedexResult()
    {
        return new PokedexResultDto
        {
            Name = PokemonName,
            Id = 1,
            Weight = 2,
            Height = 3
        };
    }
}
