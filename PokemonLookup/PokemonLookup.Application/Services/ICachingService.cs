using PokemonLookup.Domain.Entities;

namespace PokemonLookup.Application.Services;

/// <summary>
/// Used to cache requested Pokémons after it was initially requested from the Pokédex API
/// </summary>
public interface ICachingService
{
    /// <summary>
    /// Try to load a Pokémon from the cache.
    /// </summary>
    /// <param name="key">The name of the Pokémon</param>
    /// <returns>The Pokémon or null if it is not cached</returns>
    Task<Pokemon?> GetItemFromCache(string key);

    /// <summary>
    /// Save a Pokémon to the cache.
    /// </summary>
    /// <param name="item">The Pokémon to save</param>
    Task UpdateCache(Pokemon item);
}
