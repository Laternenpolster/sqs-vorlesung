using Moq;
using PokemonLookup.Web.Exceptions;
using PokemonLookup.Web.Models;
using PokemonLookup.Web.Services;

namespace TestProject1.Services;

[TestFixture]
[TestOf(typeof(PokemonLibrary))]
public class PokemonLibraryTest
{
    private const string ValidPokemonName = "abcdefg";
    private const string InvalidPokemonName = ";.-";
    
    [Test]
    public async Task TestValidItemNotInCache()
    {
        // Arrange
        var mockInputChecker = new Mock<IInputChecker>();
        mockInputChecker.Setup(service => service.IsUserInputValid(ValidPokemonName))
            .Returns(true);

        var mockApiRequester = new Mock<IPokemonApiRequester>();
        mockApiRequester.Setup(service => service.SearchByName(ValidPokemonName))
            .ReturnsAsync(GetTestPokemon());

        var mockCachingService = new Mock<ICachingService>();
        mockCachingService.Setup(service => service.GetItemFromCache(ValidPokemonName))
            .ReturnsAsync((Pokemon?) null);
        
        var library = new PokemonLibrary(mockInputChecker.Object, mockApiRequester.Object, mockCachingService.Object);
        
        // Act
        var result = await library.FetchPokemon(ValidPokemonName);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(GetTestPokemon().Name));
        
        mockApiRequester.Verify(service => service.SearchByName(ValidPokemonName), Times.Exactly(1));
        mockCachingService.Verify(service => service.UpdateCache(result), Times.Exactly(1));
    }
    
    [Test]
    public async Task TestValidItemPresentInCache()
    {
        // Arrange
        var mockInputChecker = new Mock<IInputChecker>();
        mockInputChecker.Setup(service => service.IsUserInputValid(ValidPokemonName))
            .Returns(true);

        var mockApiRequester = new Mock<IPokemonApiRequester>();
        mockApiRequester.Setup(service => service.SearchByName(ValidPokemonName))
            .ReturnsAsync(GetTestPokemon());

        var mockCachingService = new Mock<ICachingService>();
        mockCachingService.Setup(service => service.GetItemFromCache(ValidPokemonName))
            .ReturnsAsync(GetTestPokemon());
        
        var library = new PokemonLibrary(mockInputChecker.Object, mockApiRequester.Object, mockCachingService.Object);
        
        // Act
        var result = await library.FetchPokemon(ValidPokemonName);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(GetTestPokemon().Name));
        
        mockApiRequester.Verify(service => service.SearchByName(ValidPokemonName), Times.Never);
        mockCachingService.Verify(service => service.UpdateCache(result), Times.Never);
    }
    
    [Test]
    public async Task TestInvalidInputName()
    {
        // Arrange
        var mockInputChecker = new Mock<IInputChecker>();
        mockInputChecker.Setup(service => service.IsUserInputValid(InvalidPokemonName))
            .Returns(false);

        var mockApiRequester = new Mock<IPokemonApiRequester>();
        mockApiRequester.Setup(service => service.SearchByName(InvalidPokemonName))
            .ReturnsAsync(GetTestPokemon());

        var mockCachingService = new Mock<ICachingService>();
        mockCachingService.Setup(service => service.GetItemFromCache(InvalidPokemonName))
            .ReturnsAsync((Pokemon?) null);
        
        var library = new PokemonLibrary(mockInputChecker.Object, mockApiRequester.Object, mockCachingService.Object);
        
        // Act & Assert
        await Assert.ThatAsync(async () => await library.FetchPokemon(InvalidPokemonName), Throws.TypeOf<InvalidUserInputException>());

        mockApiRequester.Verify(service => service.SearchByName(InvalidPokemonName), Times.Never);
        mockCachingService.Verify(service => service.UpdateCache(It.IsAny<Pokemon>()), Times.Never);
        mockCachingService.Verify(service => service.GetItemFromCache(InvalidPokemonName), Times.Never);
    }
    
    private static Pokemon GetTestPokemon()
    {
        return new Pokemon
        {
            Name = ValidPokemonName
        };
    }
}