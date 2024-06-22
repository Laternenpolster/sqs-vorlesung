using PokemonLookup.Core.Entities;
using PokemonLookup.Web.Controllers;

namespace PokemonLookup.Web.Models;

/// <summary>
/// Used to display Pokémon information from <see cref="PokemonController"/> on the Pokémon Details page.
/// </summary>
public class PokemonResultViewModel
{
    /// <summary>
    /// The Pokémon, if it was found.
    /// </summary>
    public Pokemon? FoundPokemon { get; }

    /// <summary>
    /// If no Pokémon was found, an error message is provided.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// In image of the Pokémon, if found.
    /// </summary>
    public string? PreviewImage { get; }

    /// <summary>
    /// Response in case of a successful lookup.
    /// </summary>
    /// <param name="foundPokemon">The found Pokémon</param>
    public PokemonResultViewModel(Pokemon foundPokemon)
    {
        FoundPokemon = foundPokemon;
        PreviewImage =
            $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/home/{foundPokemon.PokemonId}.png";
    }

    /// <summary>
    /// Response in case of an unsuccessful lookup.
    /// </summary>
    /// <param name="error">The reason why no Pokémon was found.</param>
    public PokemonResultViewModel(string error)
    {
        Error = error;
    }
}
