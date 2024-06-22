using PokemonLookup.Core.Entities;
using PokemonLookup.Core.Services;

namespace PokemonLookup.LoadTests;

/// <summary>
/// An implementation of <see cref="IPokemonApiRequester"/> that only returns static data.
/// This is used to not overload the upstream Pokédex API during load tests.
/// </summary>
public class DummyApiRequester : IPokemonApiRequester
{
    private static readonly Pokemon Pikachu = new()
    {
        Name = "Pikachu",
        PokemonId = 25,
        Height = 4,
        Weight = 60
    };

    private static readonly Pokemon Ditto = new()
    {
        Name = "ditto",
        PokemonId = 25,
        Height = 4,
        Weight = 60
    };

    /// <inheritdoc/>
    public Task<Pokemon> SearchByName(string text)
    {
        var pokemon = text == "pikachu" ? Pikachu : Ditto;
        return Task.FromResult(pokemon);
    }
}
