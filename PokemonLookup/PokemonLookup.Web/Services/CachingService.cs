using PokemonLookup.Core.Entities;
using PokemonLookup.Core.Services;

namespace PokemonLookup.Web.Services;

public class CachingService(DataContext context) : ICachingService
{
    public async Task<Pokemon?> GetItemFromCache(string key)
    {
        return await context.Pokemons.FindAsync(key);
    }

    public async Task UpdateCache(Pokemon item)
    {
        context.Pokemons.Add(item);

        await context.SaveChangesAsync();
    }
}