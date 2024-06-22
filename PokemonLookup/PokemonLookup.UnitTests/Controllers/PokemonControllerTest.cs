using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Core.Services;
using PokemonLookup.Web.Controllers;
using PokemonLookup.Web.Models;
using static PokemonLookup.UnitTests.TestDataProvider;

namespace PokemonLookup.UnitTests.Controllers;

/// <summary>
/// This tests the Pokémon Details Page, which is displayed as a search result.
/// The builtin Pokémon API is tested in <see cref="PokemonApiController"/>.
/// </summary>
[TestFixture]
[TestOf(typeof(PokemonController))]
public class PokemonControllerTest
{
    /// <summary>
    /// Search for an existing Pokémon.
    /// The result should have status code 200 and all Pokémon information in the <see cref="PokemonResultViewModel"/> should be set.
    /// </summary>
    [Test]
    public async Task TestControllerWithoutError()
    {
        // Arrange
        var mockLibrary = new Mock<IPokemonLibrary>();
        mockLibrary
            .Setup(service => service.FetchPokemon(ValidPokemonName))
            .ReturnsAsync(GetValidTestPokemon());

        var controller = new PokemonController(mockLibrary.Object);

        // Act
        var result = await controller.Index(ValidPokemonName);

        // Assert
        Assert.That(result, Is.TypeOf<ViewResult>());
        var viewResult = (ViewResult)result;
        var model = (PokemonResultViewModel)viewResult.ViewData.Model!;
        Assert.Multiple(() =>
        {
            Assert.That(model.FoundPokemon, Is.Not.Null);
            Assert.That(model.PreviewImage, Is.Not.Null);
        });
        Assert.That(model.FoundPokemon.Name, Is.EqualTo(GetValidTestPokemon().Name));
    }

    /// <summary>
    /// Searches for an unknown Pokémon.
    /// A 404 status code is expected in combination with an error message.
    /// The Pokémon information in <see cref="PokemonResultViewModel"/> should not be set.
    /// </summary>
    [Test]
    public async Task TestControllerWithInvalidPokemon()
    {
        // Arrange
        var mockLibrary = new Mock<IPokemonLibrary>();
        mockLibrary
            .Setup(service => service.FetchPokemon(InvalidPokemonName))
            .Throws(() => new ApiRequestFailedException(null!, 404));

        var controller = new PokemonController(mockLibrary.Object);

        // Act
        var result = await controller.Index(InvalidPokemonName);

        // Assert
        Assert.That(result, Is.TypeOf<ViewResult>());
        var viewResult = (ViewResult)result;
        var model = (PokemonResultViewModel)viewResult.ViewData.Model!;
        Assert.Multiple(() =>
        {
            Assert.That(model.FoundPokemon, Is.Null);
            Assert.That(model.PreviewImage, Is.Null);
            Assert.That(model.Error, Is.EqualTo($"Pokemon `{InvalidPokemonName}` was not found."));
        });
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
        mockLibrary
            .Setup(service => service.FetchPokemon(InvalidPokemonName))
            .Throws(exception);

        var controller = new PokemonController(mockLibrary.Object);

        // Act
        var result = await controller.Index(InvalidPokemonName);

        // Assert
        Assert.That(result, Is.TypeOf<ViewResult>());
        var viewResult = (ViewResult)result;
        var model = (PokemonResultViewModel)viewResult.ViewData.Model!;
        Assert.Multiple(() =>
        {
            Assert.That(model.FoundPokemon, Is.Null);
            Assert.That(model.PreviewImage, Is.Null);
            Assert.That(model.Error, Is.Not.Null);
            Assert.That(model.Error, Is.EqualTo(exception.Message));
        });
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
        mockLibrary
            .Setup(service => service.FetchPokemon(InvalidPokemonName))
            .Throws(exception);

        var controller = new PokemonController(mockLibrary.Object);

        // Act
        var result = await controller.Index(InvalidPokemonName);

        // Assert
        Assert.That(result, Is.TypeOf<ViewResult>());
        var viewResult = (ViewResult)result;
        var model = (PokemonResultViewModel)viewResult.ViewData.Model!;
        Assert.Multiple(() =>
        {
            Assert.That(model.FoundPokemon, Is.Null);
            Assert.That(model.PreviewImage, Is.Null);
            Assert.That(model.Error, Is.Not.Null);
            Assert.That(model.Error, Is.EqualTo(exception.Message));
        });
    }

    /// <summary>
    /// Simulate an invalid model during the request.
    /// </summary>
    [Test]
    public async Task TestInvalidModelState()
    {
        // Arrange
        var controller = new PokemonController(null!);
        controller.ModelState.TryAddModelError("test", "test");

        // Act
        var result = await controller.Index(ValidPokemonName);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestResult>());
    }
}
