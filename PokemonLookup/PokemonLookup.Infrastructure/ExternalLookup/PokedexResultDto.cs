namespace PokemonLookup.Infrastructure.ExternalLookup;

public class PokedexResultDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int Weight { get; set; }
    public required int Height { get; set; }
}