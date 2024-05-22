using Microsoft.EntityFrameworkCore;
using PokemonLookup.Core.Entities;

namespace PokemonLookup.Infrastructure.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Pokemon> Pokemons { get; set; }
}