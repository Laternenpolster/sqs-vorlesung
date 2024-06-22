using PokemonLookup.Core.Entities;

namespace PokemonLookup.UnitTests;

/// <summary>
/// Provides static test data for all Unit Tests.
/// </summary>
public static class TestDataProvider
{
    public const string ValidPokemonName = "abcdefg";
    public const string InvalidPokemonName = ";.-";

    /// <summary>
    /// Generate a new Pokémon.
    /// </summary>
    public static Pokemon GetValidTestPokemon()
    {
        return new Pokemon
        {
            Name = ValidPokemonName,
            PokemonId = 1,
            Weight = 2,
            Height = 3
        };
    }
}
