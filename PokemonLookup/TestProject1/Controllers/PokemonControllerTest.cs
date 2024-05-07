﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonLookup.Web.Controllers;
using PokemonLookup.Web.Exceptions;
using PokemonLookup.Web.Models;
using PokemonLookup.Web.Services;
using static TestProject1.TestDataProvider;

namespace TestProject1.Controllers;

[TestFixture]
[TestOf(typeof(PokemonController))]
public class PokemonControllerTest
{
    [Test]
    public async Task TestControllerWithoutError()
    {
        // Arrange
        var mockLibrary = new Mock<IPokemonLibrary>();
        mockLibrary.Setup(service => service.FetchPokemon(ValidPokemonName))
            .ReturnsAsync(GetValidTestPokemon());

        var controller = new PokemonController(mockLibrary.Object);
        
        // Act
        var result = await controller.Index(ValidPokemonName);
        
        // Assert
        Assert.That(result, Is.TypeOf<ViewResult>());
        var viewResult = (ViewResult) result;
        var model = (PokemonResultViewModel) viewResult.ViewData.Model!;
        
        Assert.That(model.FoundPokemon, Is.Not.Null);
        Assert.That(model.FoundPokemon.Name, Is.EqualTo(GetValidTestPokemon().Name));
    }
    
    [Test]
    public async Task TestControllerWithInvalidPokemon()
    {
        // Arrange
        var mockLibrary = new Mock<IPokemonLibrary>();
        mockLibrary.Setup(service => service.FetchPokemon(InvalidPokemonName))
            .Throws(() => new ApiRequestFailedException(null!, 404));

        var controller = new PokemonController(mockLibrary.Object);
        
        // Act
        var result = await controller.Index(InvalidPokemonName);
        
        // Assert
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}