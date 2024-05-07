namespace PokemonLookup.Web.Models;

public class PokedexResult
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int Weight { get; set; }
    public required int Height { get; set; }
}