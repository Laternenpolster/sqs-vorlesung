using PokemonLookup.Core.Entities;
using PokemonLookup.Core.Services;

namespace PokemonLookup.Infrastructure.ExternalLookup;

/// <inheritdoc/>
public class PokemonApiRequester(IApiRequester apiRequester) : IPokemonApiRequester
{
    private const string RequestBaseAddress = "https://pokeapi.co/api/v2/pokemon/";

    /// <inheritdoc/>
    public async Task<Pokemon> SearchByName(string text)
    {
        var requestAddress = RequestBaseAddress + text;

        var result = await apiRequester.GetRequest<PokedexResultDto>(requestAddress);

        // Convert the DTO to the Domain entity
        return new Pokemon
        {
            Name = result.Name,
            PokemonId = result.Id,
            Weight = result.Weight,
            Height = result.Height
        };
    }
}
