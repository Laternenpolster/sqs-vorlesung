using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonLookup.Core.Entities;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Core.Services;
using PokemonLookup.Web.Controllers;
using static PokemonLookup.UnitTests.TestDataProvider;

namespace PokemonLookup.UnitTests.Controllers;

/// <summary>
/// Tests the builtin Pokémon API.
/// The website is tested in <see cref="PokemonController"/>.
/// </summary>
public class PokemonApiControllerTest
{
    /// <summary>
    /// Tries to search an existing Pokémon with the builtin API.
    /// This should return the Pokémon with a 200 status code.
    /// </summary>
    [Fact]
    public async Task TestControllerWithoutError()
    {
        // Arrange
        var mockLibrary = new Mock<IPokemonLibrary>();
        mockLibrary
            .Setup(service => service.FetchPokemon(ValidPokemonName))
            .ReturnsAsync(GetValidTestPokemon());

        var controller = new PokemonApiController(mockLibrary.Object);

        // Act
        var result = await controller.GetByName(ValidPokemonName);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var viewResult = (OkObjectResult)result;
        var model = (Pokemon)viewResult.Value!;

        Assert.NotNull(model);
        Assert.Equal(GetValidTestPokemon().Name, model.Name);
    }

    /// <summary>
    /// Tries to search a Pokémon that does not exist.
    /// A 404 result with a specific error message is expected.
    /// </summary>
    [Fact]
    public async Task TestControllerWithInvalidPokemon()
    {
        // Arrange
        var exception = new ApiRequestFailedException(null!, 404);

        var mockLibrary = new Mock<IPokemonLibrary>();
        mockLibrary
            .Setup(service => service.FetchPokemon(InvalidPokemonName))
            .Throws(exception);

        var controller = new PokemonApiController(mockLibrary.Object);

        // Act
        var result = await controller.GetByName(InvalidPokemonName);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var httpResult = (NotFoundObjectResult)result;
        var errorMessage = (string?)httpResult.Value;

        const string expectedError = $"Pokemon `{InvalidPokemonName}` was not found.";
        Assert.Equal(expectedError, errorMessage);
    }

    /// <summary>
    /// Simulates a request with an invalid Pokémon name.
    /// The API should return a 400 status code with an error message.
    /// </summary>
    [Fact]
    public async Task TestControllerWithInvalidInput()
    {
        // Arrange
        var exception = new InvalidUserInputException(InvalidPokemonName);

        var mockLibrary = new Mock<IPokemonLibrary>();
        mockLibrary.Setup(service => service.FetchPokemon(InvalidPokemonName))
            .Throws(exception);

        var controller = new PokemonApiController(mockLibrary.Object);

        // Act
        var result = await controller.GetByName(InvalidPokemonName);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var httpResult = (BadRequestObjectResult)result;
        var errorMessage = (string?)httpResult.Value;

        Assert.Equal(exception.Message, errorMessage);
    }

    /// <summary>
    /// This tests simulates an exception in the Pokédex Lookup.
    /// This should result in a 500 status code with a generic error message.
    /// </summary>
    [Fact]
    public async Task TestControllerWithHttpRequestException()
    {
        // Arrange
        var exception = new ApiRequestFailedException(null!, 401);

        var mockLibrary = new Mock<IPokemonLibrary>();
        mockLibrary.Setup(service => service.FetchPokemon(InvalidPokemonName))
            .Throws(exception);

        var controller = new PokemonApiController(mockLibrary.Object);

        // Act
        var result = await controller.GetByName(InvalidPokemonName);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var viewResult = (ObjectResult)result;

        Assert.Equal(500, viewResult.StatusCode);
        Assert.Equal(exception.Message, viewResult.Value);
    }
}
