using Microsoft.AspNetCore.Mvc;
using PokemonLookup.Application.Exceptions;
using PokemonLookup.Application.Services;

namespace PokemonLookup.Web.Controllers;

/// <summary>
/// A simple API to request Pokémon from cache or the Pokédex API.
/// </summary>
/// <param name="library">Logic how the Pokémon should be resolved</param>
[ApiController]
[Route("/api/v1/pokemon")]
public class PokemonApiController(IPokemonLibrary library) : ControllerBase
{
    /// <summary>
    /// Find a Pokémon based on its name.
    /// </summary>
    /// <param name="name">The name of the Pokémon</param>
    /// <returns>
    /// 200 when the Pokémon was found.
    /// 400 when the user input is invalid
    /// 404 when it was not found in cache and the Pokédex.
    /// </returns>
    [HttpGet]
    [Route("{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        try
        {
            // Find it in cache or the Pokédex
            var apiResult = await library.FetchPokemon(name);

            return Ok(apiResult);
        }
        // The name did not pass the filter
        catch (InvalidUserInputException ex)
        {
            return BadRequest(ex.Message);
        }
        // The Pokémon was not found in either the cache or the Pokédex
        catch (ApiRequestFailedException requestFailedException)
            when (requestFailedException.ErrorCode == 404)
        {
            return NotFound($"Pokemon `{name}` was not found.");
        }
        // An error occured while requesting the Pokédex
        catch (ApiRequestFailedException requestFailedException)
        {
            return StatusCode(500, requestFailedException.Message);
        }
    }
}
