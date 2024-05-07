using PokemonLookup.Web.Models;

namespace TestProject1;

public static class TestDataProvider
{
    public const string ValidPokemonName = "abcdefg";
    public const string InvalidPokemonName = ";.-";
    
    public static Pokemon GetValidTestPokemon()
    {
        return new Pokemon
        {
            Name = ValidPokemonName
        };
    }
}