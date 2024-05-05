using PokemonLookup.Web.Models;

namespace PokemonLookup.Web.Services;

public class PokemonApiRequester(IApiRequester apiRequester) : IPokemonApiRequester
{
    private const string RequestBaseAddress = "https://pokeapi.co/api/v2/pokemon/";

    public async Task<Pokemon> SearchByName(string text)
    {
        var requestAddress = RequestBaseAddress + text;
        
        var result = await apiRequester.GetRequest<PokedexResult>(requestAddress);
        
        return new Pokemon
        {
            Name = result.Name
        };
    }
}