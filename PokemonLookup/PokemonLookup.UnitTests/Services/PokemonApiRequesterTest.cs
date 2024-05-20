using Moq;
using PokemonLookup.Core.Services;
using PokemonLookup.Web.Models;
using PokemonLookup.Web.Services;

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
        mockApi.Setup(service => service.GetRequest<PokedexResult>(It.IsAny<string>()))
            .ReturnsAsync(GetTestPokedexResult());
        
        var apiRequester = new PokemonApiRequester(mockApi.Object);
        
        // Act
        var result = await apiRequester.SearchByName(PokemonName);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(GetTestPokedexResult().Name));
    }
    
    private static PokedexResult GetTestPokedexResult()
    {
        return new PokedexResult
        {
            Name = PokemonName,
            Id = 1,
            Weight = 2,
            Height = 3
        };
    }
}