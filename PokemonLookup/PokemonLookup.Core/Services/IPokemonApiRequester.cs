using PokemonLookup.Core.Entities;

namespace PokemonLookup.Core.Services;

/// <summary>
/// Requests a Pokémon from the Pokédex.
/// This is used if a Pokémon is not cached yet.
/// </summary>
public interface IPokemonApiRequester
{
    /// <summary>
    /// Requests a Pokémon from the Pokédex.
    /// This is used if a Pokémon is not cached yet.
    /// </summary>
    /// <param name="text">The name of the Pokémon that should be searched</param>
    /// <returns>The Pokémon or an exception if the Pokémon is unknown</returns>
    Task<Pokemon> SearchByName(string text);
}
