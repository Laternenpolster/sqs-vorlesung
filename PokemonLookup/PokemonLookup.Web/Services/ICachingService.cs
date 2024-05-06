using PokemonLookup.Web.Models;

namespace PokemonLookup.Web.Services;

public interface ICachingService
{
    Task<Pokemon?> GetItemFromCache(string key);
    Task UpdateCache(Pokemon item);
}