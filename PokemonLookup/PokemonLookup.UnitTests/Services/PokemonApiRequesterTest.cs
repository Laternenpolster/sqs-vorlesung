using Moq;
using PokemonLookup.Core.Services;
using PokemonLookup.Infrastructure.ExternalLookup;

namespace PokemonLookup.UnitTests.Services;

[TestFixture]
[TestOf(typeof(PokemonApiRequester))]
public class PokemonApiRequesterTest
{
    private const string PokemonName = "abcdefg";

    [Test]
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
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(GetTestPokedexResult().Name));
    }

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
