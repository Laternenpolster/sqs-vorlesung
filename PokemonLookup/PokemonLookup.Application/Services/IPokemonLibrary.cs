using PokemonLookup.Domain.Entities;

namespace PokemonLookup.Application.Services;

/// <summary>
/// Contains all the business logic for finding a Pokémon.
/// It handles requests to the Pokédex API and saves results to the cache.
/// </summary>
public interface IPokemonLibrary
{
    /// <summary>
    /// Load a Pokémon from cache or the Pokédex API.
    /// </summary>
    /// <param name="name">The name of the Pokémon</param>
    /// <returns>The Pokémon or an exception if the Pokémon is unknown</returns>
    Task<Pokemon> FetchPokemon(string name);
}
