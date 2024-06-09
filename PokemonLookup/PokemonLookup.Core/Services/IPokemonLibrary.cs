using PokemonLookup.Core.Entities;

namespace PokemonLookup.Core.Services;

public interface IPokemonLibrary
{
    Task<Pokemon> FetchPokemon(string name);
}