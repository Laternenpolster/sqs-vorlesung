using System.Net;
using PokemonLookup.Core.Entities;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Infrastructure.ExternalLookup;
using RichardSzalay.MockHttp;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static PokemonLookup.UnitTests.TestDataProvider;

namespace PokemonLookup.UnitTests.Services;

[TestFixture]
[TestOf(typeof(ApiRequester))]
public class ApiRequesterTest
{
    private const string TestUrl = "https://google.com";
    private const string ContentType = "application/json";
    
    [Test]
    public async Task TestValidRequest()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(TestUrl)
            .Respond(ContentType, GetValidHttpResponse());

        var httpClient = mockHttp.ToHttpClient();
        
        var apiRequester = new ApiRequester(httpClient);
        
        // Act
        var result = await apiRequester.GetRequest<Pokemon>(TestUrl);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(GetValidTestPokemon().Name));
    }
    
    [Test]
    public async Task TestNotFoundException()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(TestUrl)
            .Respond(HttpStatusCode.NotFound);
    
        var httpClient = mockHttp.ToHttpClient();
        
        var apiRequester = new ApiRequester(httpClient);
        
        // Act & Assert
        try
        {
            await apiRequester.GetRequest<Pokemon>(TestUrl);
            
            Assert.Fail("Expected `ApiRequestFailedException` exception.");
        }
        catch (ApiRequestFailedException exception)
        {
            Assert.That(exception.ErrorCode, Is.EqualTo(404));
        }
    }
    
    [Test]
    public async Task TestGenericException()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(TestUrl)
            .Respond(ContentType, string.Empty);
    
        var httpClient = mockHttp.ToHttpClient();
        
        var apiRequester = new ApiRequester(httpClient);
        
        // Act & Assert
        try
        {
            await apiRequester.GetRequest<Pokemon>(TestUrl);
            
            Assert.Fail("Expected `ApiRequestFailedException` exception.");
        }
        catch (ApiRequestFailedException exception)
        {
            Assert.That(exception.ErrorCode, Is.EqualTo(-1));
        }
    }
    
    private static string GetValidHttpResponse()
    {
        var testObject = GetValidTestPokemon();
        return JsonSerializer.Serialize(testObject);
    }
}