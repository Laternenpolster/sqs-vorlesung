using Microsoft.AspNetCore.Mvc;
using PokemonLookup.Web.Exceptions;
using PokemonLookup.Web.Models;
using PokemonLookup.Web.Services;

namespace PokemonLookup.Web.Controllers;

public class PokemonController(IPokemonApiRequester apiRequester) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string name)
    {
        try
        {
            var apiResult = await apiRequester.SearchByName(name);
            var result = new PokemonResultViewModel
            {
                FoundPokemon = apiResult
            };

            return View(result);
        }
        catch (ApiRequestFailedException requestFailedException) when(requestFailedException.ErrorCode == 404)
        {
            return NotFound();
        } 
    }
}