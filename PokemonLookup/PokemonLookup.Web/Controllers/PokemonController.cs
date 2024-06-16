using System.Net;
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
        PokemonResultViewModel viewModel;
        HttpStatusCode resultStatus;
        try
        {
            var apiResult = await library.FetchPokemon(name);

            viewModel = new PokemonResultViewModel(apiResult);
            resultStatus = HttpStatusCode.OK;
        }
        catch (InvalidUserInputException ex)
        {
            viewModel = new PokemonResultViewModel(ex.Message);
            resultStatus = HttpStatusCode.BadRequest;
        }
        catch (ApiRequestFailedException requestFailedException) when(requestFailedException.ErrorCode == 404)
        {
            viewModel = new PokemonResultViewModel($"Pokemon `{name}` was not found.");
            resultStatus = HttpStatusCode.NotFound;
        }
        catch (ApiRequestFailedException requestFailedException)
        {
            viewModel = new PokemonResultViewModel(requestFailedException.Message);
            resultStatus = HttpStatusCode.InternalServerError;
        }

        var result = View(viewModel);
        result.StatusCode = (int) resultStatus;
        return result;
    }
}
