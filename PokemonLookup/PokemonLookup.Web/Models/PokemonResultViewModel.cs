using PokemonLookup.Core.Entities;

namespace PokemonLookup.Web.Models;

public class PokemonResultViewModel
{
    public Pokemon? FoundPokemon { get; }
    public string? Error { get; }
    public string? PreviewImage { get; }

    public PokemonResultViewModel(Pokemon foundPokemon)
    {
        FoundPokemon = foundPokemon;
        PreviewImage =
            $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/home/{foundPokemon.PokemonId}.png";
    }

    public PokemonResultViewModel(string error)
    {
        Error = error;
    }
}
