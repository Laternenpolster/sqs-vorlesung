using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using PokemonLookup.Application.Services;
using PokemonLookup.Domain.Entities;
using PokemonLookup.Infrastructure.Data;

namespace PokemonLookup.Infrastructure.Caching;

/// <inheritdoc/>
public class DatabaseCachingService(DataContext context) : ICachingService
{
    /// <inheritdoc/>
    public async Task<Pokemon?> GetItemFromCache(string key)
    {
        return await context.Pokemons.FindAsync(key);
    }

    /// <inheritdoc/>
    public async Task UpdateCache(Pokemon item)
    {
        try
        {
            context.Pokemons.Add(item);

            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is DbException { SqlState: "23505" })
        {
            // Happens when the same Pokémon is inserted concurrently.
            // As the Pokémon did already exist, ignore the failed insert
        }
    }
}
