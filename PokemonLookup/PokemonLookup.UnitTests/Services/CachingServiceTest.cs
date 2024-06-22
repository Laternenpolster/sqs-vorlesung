using Microsoft.EntityFrameworkCore;
using PokemonLookup.Infrastructure;
using PokemonLookup.Infrastructure.Data;

namespace PokemonLookup.UnitTests.Services;

/// <summary>
/// Tests the Pokémon caching.
/// </summary>
[TestFixture]
[TestOf(typeof(CachingService))]
public class CachingServiceTest
{
    /// <summary>
    /// Try to save a new Pokémon in the cache and check if it is present afterward.
    /// </summary>
    [Test]
    public async Task TestSaveNewPokemon()
    {
        // Arrange
        var dbContextOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(nameof(TestSaveNewPokemon))
            .Options;

        await using var mockContext = new DataContext(dbContextOptions);
        var cachingService = new CachingService(mockContext);

        var testPokemon = TestDataProvider.GetValidTestPokemon();

        // Act
        var previouslyCachedPokemon = await cachingService.GetItemFromCache(testPokemon.Name);

        await cachingService.UpdateCache(testPokemon);

        var newlyCachedPokemon = await cachingService.GetItemFromCache(testPokemon.Name);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(previouslyCachedPokemon, Is.Null);
            Assert.That(newlyCachedPokemon, Is.Not.Null);
            Assert.That(newlyCachedPokemon, Is.EqualTo(testPokemon));
        });
    }
}
