using Microsoft.EntityFrameworkCore;

namespace PokemonLookup.Web.Models;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Pokemon> Pokemons { get; set; }
}