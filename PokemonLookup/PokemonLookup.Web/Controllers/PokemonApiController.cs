using Microsoft.AspNetCore.Mvc;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Core.Services;

namespace PokemonLookup.Web.Controllers;

[ApiController]
[Route("/api/v1/pokemon")]
public class PokemonApiController(IPokemonLibrary library) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByName(string name)
    {
        try
        {
            var apiResult = await library.FetchPokemon(name);

            return Ok(apiResult);
        }
        catch (InvalidUserInputException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ApiRequestFailedException requestFailedException)
            when (requestFailedException.ErrorCode == 404)
        {
            return NotFound($"Pokemon `{name}` was not found.");
        }
        catch (ApiRequestFailedException requestFailedException)
        {
            return StatusCode(500, requestFailedException.Message);
        }
    }
}
