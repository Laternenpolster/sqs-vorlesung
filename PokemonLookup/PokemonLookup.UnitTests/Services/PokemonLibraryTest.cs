using Moq;
using PokemonLookup.Core.Entities;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Core.Services;
using PokemonLookup.Infrastructure.ExternalLookup;
using static PokemonLookup.UnitTests.TestDataProvider;

namespace PokemonLookup.UnitTests.Services;

[TestFixture]
[TestOf(typeof(PokemonLibrary))]
public class PokemonLibraryTest
{
    [Test]
    public async Task TestValidItemNotInCache()
    {
        // Arrange
        var mockInputChecker = new Mock<IInputChecker>();
        mockInputChecker
            .Setup(service => service.IsUserInputValid(ValidPokemonName))
            .Returns(true);

        var mockApiRequester = new Mock<IPokemonApiRequester>();
        mockApiRequester
            .Setup(service => service.SearchByName(ValidPokemonName))
            .ReturnsAsync(GetValidTestPokemon());

        var mockCachingService = new Mock<ICachingService>();
        mockCachingService
            .Setup(service => service.GetItemFromCache(ValidPokemonName))
            .ReturnsAsync((Pokemon?)null);

        var library = new PokemonLibrary(mockInputChecker.Object, mockApiRequester.Object, mockCachingService.Object);

        // Act
        var result = await library.FetchPokemon(ValidPokemonName);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(GetValidTestPokemon().Name));

        mockApiRequester.Verify(service => service.SearchByName(ValidPokemonName), Times.Exactly(1));
        mockCachingService.Verify(service => service.UpdateCache(result), Times.Exactly(1));
    }

    [Test]
    public async Task TestValidItemPresentInCache()
    {
        // Arrange
        var mockInputChecker = new Mock<IInputChecker>();
        mockInputChecker
            .Setup(service => service.IsUserInputValid(ValidPokemonName))
            .Returns(true);

        var mockApiRequester = new Mock<IPokemonApiRequester>();
        mockApiRequester
            .Setup(service => service.SearchByName(ValidPokemonName))
            .ReturnsAsync(GetValidTestPokemon());

        var mockCachingService = new Mock<ICachingService>();
        mockCachingService
            .Setup(service => service.GetItemFromCache(ValidPokemonName))
            .ReturnsAsync(GetValidTestPokemon());

        var library = new PokemonLibrary(mockInputChecker.Object, mockApiRequester.Object, mockCachingService.Object);

        // Act
        var result = await library.FetchPokemon(ValidPokemonName);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(GetValidTestPokemon().Name));

        mockApiRequester.Verify(service => service.SearchByName(ValidPokemonName), Times.Never);
        mockCachingService.Verify(service => service.UpdateCache(result), Times.Never);
    }

    [Test]
    public async Task TestInvalidInputName()
    {
        // Arrange
        var mockInputChecker = new Mock<IInputChecker>();
        mockInputChecker
            .Setup(service => service.IsUserInputValid(InvalidPokemonName))
            .Returns(false);

        var mockApiRequester = new Mock<IPokemonApiRequester>();
        mockApiRequester
            .Setup(service => service.SearchByName(InvalidPokemonName))
            .ReturnsAsync(GetValidTestPokemon());

        var mockCachingService = new Mock<ICachingService>();
        mockCachingService
            .Setup(service => service.GetItemFromCache(InvalidPokemonName))
            .ReturnsAsync((Pokemon?)null);

        var library = new PokemonLibrary(mockInputChecker.Object, mockApiRequester.Object, mockCachingService.Object);

        // Act & Assert
        await Assert.ThatAsync(async () => await library.FetchPokemon(InvalidPokemonName), Throws.TypeOf<InvalidUserInputException>());

        mockApiRequester.Verify(service => service.SearchByName(InvalidPokemonName), Times.Never);
        mockCachingService.Verify(service => service.UpdateCache(It.IsAny<Pokemon>()), Times.Never);
        mockCachingService.Verify(service => service.GetItemFromCache(InvalidPokemonName), Times.Never);
    }
}
