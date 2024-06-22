using PokemonLookup.Core.Entities;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Core.Services;

namespace PokemonLookup.Infrastructure.ExternalLookup;

/// <inheritdoc/>
public class PokemonLibrary : IPokemonLibrary
{
    private readonly IInputChecker _inputChecker;
    private readonly IPokemonApiRequester _apiRequester;
    private readonly ICachingService _cachingService;

    /// <inheritdoc cref="PokemonLibrary" />
    public PokemonLibrary(IInputChecker inputChecker, IPokemonApiRequester apiRequester, ICachingService cachingService)
    {
        _inputChecker = inputChecker;
        _apiRequester = apiRequester;
        _cachingService = cachingService;
    }

    /// <inheritdoc/>
    public async Task<Pokemon> FetchPokemon(string name)
    {
        // Check the user input first
        if (!_inputChecker.IsUserInputValid(name))
        {
            throw new InvalidUserInputException("Please only use letters and numbers.");
        }

        // Check if the Pokémon is cached
        var cacheResult = await _cachingService.GetItemFromCache(name);
        if (cacheResult != null)
        {
            return cacheResult;
        }

        // Request the Pokédex if the Pokémon is not cached yet
        var apiResult = await _apiRequester.SearchByName(name);
        await _cachingService.UpdateCache(apiResult);

        return apiResult;
    }
}
