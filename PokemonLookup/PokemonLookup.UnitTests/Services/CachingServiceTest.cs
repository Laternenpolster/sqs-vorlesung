using Microsoft.EntityFrameworkCore;
using PokemonLookup.Infrastructure.Caching;
using PokemonLookup.Infrastructure.Data;

namespace PokemonLookup.UnitTests.Services;

/// <summary>
/// Tests the Pokémon caching.
/// </summary>
public class CachingServiceTest
{
    /// <summary>
    /// Try to save a new Pokémon in the cache and check if it is present afterward.
    /// </summary>
    [Fact]
    public async Task TestSaveNewPokemon()
    {
        // Arrange
        var dbContextOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(nameof(TestSaveNewPokemon))
            .Options;

        await using var mockContext = new DataContext(dbContextOptions);
        var cachingService = new DatabaseCachingService(mockContext);

        var testPokemon = TestDataProvider.GetValidTestPokemon();

        // Act
        var previouslyCachedPokemon = await cachingService.GetItemFromCache(testPokemon.Name);

        await cachingService.UpdateCache(testPokemon);

        var newlyCachedPokemon = await cachingService.GetItemFromCache(testPokemon.Name);

        // Assert
        Assert.Null(previouslyCachedPokemon);
        Assert.NotNull(newlyCachedPokemon);
        Assert.Equal(testPokemon, newlyCachedPokemon);
    }
}
