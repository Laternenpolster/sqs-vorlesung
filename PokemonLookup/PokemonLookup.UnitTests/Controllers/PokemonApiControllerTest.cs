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
[TestFixture]
[TestOf(typeof(PokemonApiController))]
public class PokemonApiControllerTest
{
    /// <summary>
    /// Tries to search an existing Pokémon with the builtin API.
    /// This should return the Pokémon with a 200 status code.
    /// </summary>
    [Test]
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
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var viewResult = (OkObjectResult)result;
        var model = (Pokemon)viewResult.Value!;

        Assert.That(model, Is.Not.Null);
        Assert.That(model.Name, Is.EqualTo(GetValidTestPokemon().Name));
    }

    /// <summary>
    /// Tries to search a Pokémon that does not exist.
    /// A 404 result with a specific error message is expected.
    /// </summary>
    [Test]
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
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        var httpResult = (NotFoundObjectResult)result;
        var errorMessage = (string?)httpResult.Value;

        const string expectedError = $"Pokemon `{InvalidPokemonName}` was not found.";
        Assert.That(errorMessage, Is.EqualTo(expectedError));
    }

    /// <summary>
    /// Simulates a request with an invalid Pokémon name.
    /// The API should return a 400 status code with an error message.
    /// </summary>
    [Test]
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
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        var httpResult = (BadRequestObjectResult)result;
        var errorMessage = (string?)httpResult.Value;

        Assert.That(errorMessage, Is.EqualTo(exception.Message));
    }

    /// <summary>
    /// This tests simulates an exception in the Pokédex Lookup.
    /// This should result in a 500 status code with a generic error message.
    /// </summary>
    [Test]
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
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var viewResult = (ObjectResult)result;

        Assert.Multiple(() =>
        {
            Assert.That(viewResult.StatusCode, Is.EqualTo(500));
            Assert.That(viewResult.Value, Is.EqualTo(exception.Message));
        });
    }
}
