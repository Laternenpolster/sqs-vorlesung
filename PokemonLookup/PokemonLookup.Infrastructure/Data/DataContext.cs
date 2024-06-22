using Microsoft.EntityFrameworkCore;
using PokemonLookup.Domain.Entities;

namespace PokemonLookup.Infrastructure.Data;

/// <summary>
/// The Entity Framework connection to the database.
/// </summary>
/// <param name="options">Database configuration</param>
public class DataContext(DbContextOptions options) : DbContext(options)
{
    /// <summary>
    /// The only relation used by this application: Pokémons
    /// </summary>
    public DbSet<Pokemon> Pokemons { get; set; }
}
