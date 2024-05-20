using Microsoft.AspNetCore.Mvc;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Core.Services;
using PokemonLookup.Web.Models;

namespace PokemonLookup.Web.Controllers;

public class PokemonController(IPokemonLibrary library) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string name)
    {
        PokemonResultViewModel result;
        try
        {
            var apiResult = await library.FetchPokemon(name);
            result = new PokemonResultViewModel(apiResult);
        }
        catch (InvalidUserInputException ex)
        {
            result = new PokemonResultViewModel(ex.Message);
        }
        catch (ApiRequestFailedException requestFailedException) when(requestFailedException.ErrorCode == 404)
        {
            result = new PokemonResultViewModel($"Pokemon `{name}` was not found.");
        }
        catch (ApiRequestFailedException requestFailedException)
        {
            result = new PokemonResultViewModel(requestFailedException.Message);
        } 
        
        return View(result);
    }
}