using Microsoft.EntityFrameworkCore;
using PokemonLookup.Core.Entities;

namespace PokemonLookup.Web.Models;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Pokemon> Pokemons { get; set; }
}