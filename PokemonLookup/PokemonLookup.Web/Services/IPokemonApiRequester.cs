using PokemonLookup.Web.Models;

namespace PokemonLookup.Web.Services;

public interface IPokemonApiRequester
{
    Task<Pokemon> SearchByName(string text);
}