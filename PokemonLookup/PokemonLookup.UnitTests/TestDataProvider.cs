using PokemonLookup.Core.Entities;

namespace PokemonLookup.UnitTests;

public static class TestDataProvider
{
    public const string ValidPokemonName = "abcdefg";
    public const string InvalidPokemonName = ";.-";
    
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