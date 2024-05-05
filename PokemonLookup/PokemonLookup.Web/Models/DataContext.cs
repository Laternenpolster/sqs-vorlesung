using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Pokemon> Pokemons { get; set; }
}