using System.ComponentModel.DataAnnotations;

namespace PokemonLookup.Core.Entities;

public class Pokemon
{
    [Key]
    [MaxLength(20)]
    public required string Name { get; set; }

    public required int PokemonId { get; set; }
    public required int Height { get; set; }
    public required int Weight { get; set; }
}
