using PokemonLookup.Web.Exceptions;
using PokemonLookup.Web.Models;

namespace PokemonLookup.Web.Services;

public class PokemonLibrary : IPokemonLibrary
{
    private readonly IInputChecker _inputChecker;
    private readonly IPokemonApiRequester _apiRequester;
    private readonly ICachingService _cachingService;

    public PokemonLibrary(IInputChecker inputChecker, IPokemonApiRequester apiRequester, ICachingService cachingService)
    {
        _inputChecker = inputChecker;
        _apiRequester = apiRequester;
        _cachingService = cachingService;
    }

    public async Task<Pokemon> FetchPokemon(string name)
    {
        if (!_inputChecker.IsUserInputValid(name))
        {
            throw new InvalidUserInputException("Please only use letters and numbers.");
        }

        var cacheResult = await _cachingService.GetItemFromCache(name);
        if (cacheResult != null)
        {
            return cacheResult;
        }

        var apiResult = await _apiRequester.SearchByName(name);
        await _cachingService.UpdateCache(apiResult);

        return apiResult;
    }
}