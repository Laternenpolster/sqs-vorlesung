using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonLookup.Application.Exceptions;
using PokemonLookup.Application.Services;
using PokemonLookup.Web.Controllers;
using PokemonLookup.Web.Models;
using static PokemonLookup.UnitTests.TestDataProvider;

namespace PokemonLookup.UnitTests.Controllers;

/// <summary>
/// This tests the Pokémon Details Page, which is displayed as a search result.
/// The builtin Pokémon API is tested in <see cref="PokemonApiController"/>.
/// </summary>
public class PokemonControllerTest
{
    /// <summary>
    /// Search for an existing Pokémon.
    /// The result should have status code 200 and all Pokémon information in the <see cref="PokemonResultViewModel"/> should be set.
    /// </summary>
    [Fact]
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
        Assert.IsType<ViewResult>(result);
        var viewResult = (ViewResult)result;
        var model = (PokemonResultViewModel)viewResult.ViewData.Model!;

        Assert.NotNull(model.FoundPokemon);
        Assert.NotNull(model.PreviewImage);
        Assert.Equal(GetValidTestPokemon().Name, model.FoundPokemon.Name);
    }

    /// <summary>
    /// Searches for an unknown Pokémon.
    /// A 404 status code is expected in combination with an error message.
    /// The Pokémon information in <see cref="PokemonResultViewModel"/> should not be set.
    /// </summary>
    [Fact]
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
        Assert.IsType<ViewResult>(result);
        var viewResult = (ViewResult)result;
        var model = (PokemonResultViewModel)viewResult.ViewData.Model!;

        Assert.Null(model.FoundPokemon);
        Assert.Null(model.PreviewImage);
        Assert.Equal($"Pokemon `{InvalidPokemonName}` was not found.", model.Error);
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
        mockLibrary
            .Setup(service => service.FetchPokemon(InvalidPokemonName))
            .Throws(exception);

        var controller = new PokemonController(mockLibrary.Object);

        // Act
        var result = await controller.Index(InvalidPokemonName);

        // Assert
        Assert.IsType<ViewResult>(result);
        var viewResult = (ViewResult)result;
        var model = (PokemonResultViewModel)viewResult.ViewData.Model!;

        Assert.Null(model.FoundPokemon);
        Assert.Null(model.PreviewImage);
        Assert.NotNull(model.Error);
        Assert.Equal(exception.Message, model.Error);
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
        mockLibrary
            .Setup(service => service.FetchPokemon(InvalidPokemonName))
            .Throws(exception);

        var controller = new PokemonController(mockLibrary.Object);

        // Act
        var result = await controller.Index(InvalidPokemonName);

        // Assert
        Assert.IsType<ViewResult>(result);
        var viewResult = (ViewResult)result;
        var model = (PokemonResultViewModel)viewResult.ViewData.Model!;

        Assert.Null(model.FoundPokemon);
        Assert.Null(model.PreviewImage);
        Assert.NotNull(model.Error);
        Assert.Equal(exception.Message, model.Error);
    }

    /// <summary>
    /// Simulate an invalid model during the request.
    /// </summary>
    [Fact]
    public async Task TestInvalidModelState()
    {
        // Arrange
        var controller = new PokemonController(null!);
        controller.ModelState.TryAddModelError("test", "test");

        // Act
        var result = await controller.Index(ValidPokemonName);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}
