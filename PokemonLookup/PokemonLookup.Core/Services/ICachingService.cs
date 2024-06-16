using PokemonLookup.Core.Entities;

namespace PokemonLookup.Core.Services;

public interface ICachingService
{
    Task<Pokemon?> GetItemFromCache(string key);
    Task UpdateCache(Pokemon item);
}
