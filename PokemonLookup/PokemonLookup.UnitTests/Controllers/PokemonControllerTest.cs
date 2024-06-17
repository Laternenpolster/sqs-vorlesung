using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Core.Services;
using PokemonLookup.Web.Controllers;
using PokemonLookup.Web.Models;
using static PokemonLookup.UnitTests.TestDataProvider;

namespace PokemonLookup.UnitTests.Controllers;

[TestFixture]
[TestOf(typeof(PokemonController))]
public class PokemonControllerTest
{
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
