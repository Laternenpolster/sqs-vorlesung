using PokemonLookup.Core.Entities;
using PokemonLookup.Core.Services;
using PokemonLookup.Infrastructure.Data;

namespace PokemonLookup.Infrastructure;

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