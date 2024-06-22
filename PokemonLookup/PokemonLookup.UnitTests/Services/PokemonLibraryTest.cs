using Moq;
using PokemonLookup.Application.Exceptions;
using PokemonLookup.Application.Services;
using PokemonLookup.Domain.Entities;
using PokemonLookup.Infrastructure.ExternalLookup;
using static PokemonLookup.UnitTests.TestDataProvider;

namespace PokemonLookup.UnitTests.Services;

/// <summary>
/// Tests the logic that combines caching and Pokédex API lookups.
/// </summary>
public class PokemonLibraryTest
{
    /// <summary>
    /// Simulates a lookup for a Pokémon that was not cached.
    /// It should be requested from the Pokédex API and stored in the cache.
    /// </summary>
    [Fact]
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
        Assert.NotNull(result);
        Assert.Equal(GetValidTestPokemon().Name, result.Name);

        mockApiRequester.Verify(service => service.SearchByName(ValidPokemonName), Times.Exactly(1));
        mockCachingService.Verify(service => service.UpdateCache(result), Times.Exactly(1));
    }

    /// <summary>
    /// Simulates a lookup for a Pokémon that is already cached.
    /// The Pokédex API should not be used, only the cache.
    /// </summary>
    [Fact]
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
        Assert.NotNull(result);
        Assert.Equal(GetValidTestPokemon().Name, result.Name);

        mockApiRequester.Verify(service => service.SearchByName(ValidPokemonName), Times.Never);
        mockCachingService.Verify(service => service.UpdateCache(result), Times.Never);
    }

    /// <summary>
    /// Simulate a lookup for a Pokémon with an invalid name.
    /// The service should throw a <see cref="InvalidUserInputException"/> and not access cache or API.
    /// </summary>
    [Fact]
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
        await Assert.ThrowsAsync<InvalidUserInputException>(async () => await library.FetchPokemon(InvalidPokemonName));

        mockApiRequester.Verify(service => service.SearchByName(InvalidPokemonName), Times.Never);
        mockCachingService.Verify(service => service.UpdateCache(It.IsAny<Pokemon>()), Times.Never);
        mockCachingService.Verify(service => service.GetItemFromCache(InvalidPokemonName), Times.Never);
    }
}
