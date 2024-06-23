namespace PokemonLookup.Infrastructure.ExternalLookup;

/// <summary>
/// Result DTO returned by the Pokédex API used in <see cref="PokemonApiRequester"/>.
/// </summary>
public class PokedexResultDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int Weight { get; set; }
    public required int Height { get; set; }
}
