using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IPokemonApiRequester
{
    Task<Pokemon> SearchByName(string text);
}