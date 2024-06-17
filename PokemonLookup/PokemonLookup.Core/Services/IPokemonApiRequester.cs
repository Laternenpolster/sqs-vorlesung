using PokemonLookup.Core.Entities;

namespace PokemonLookup.Core.Services;

public interface IPokemonApiRequester
{
    Task<Pokemon> SearchByName(string text);
}
