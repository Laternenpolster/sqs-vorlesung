using PokemonLookup.Web.Models;

namespace PokemonLookup.Web.Services;

public interface IPokemonLibrary
{
    Task<Pokemon> FetchPokemon(string name);
}