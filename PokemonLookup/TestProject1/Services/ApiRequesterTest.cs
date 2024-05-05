using PokemonLookup.Web.Models;
using PokemonLookup.Web.Services;
using RichardSzalay.MockHttp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TestProject1.Services;

[TestFixture]
[TestOf(typeof(ApiRequester))]
public class ApiRequesterTest
{
    private const string PokemonName = "abcdefg";
    private const string TestUrl = "https://google.com";
    private const string ContentType = "application/json";
    
    [Test]
    public async Task TestValidRequest()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(TestUrl)
            .Respond(ContentType, GetTestHttpResponse());

        var httpClient = mockHttp.ToHttpClient();
        
        var apiRequester = new ApiRequester(httpClient);
        
        // Act
        var result = await apiRequester.GetRequest<Pokemon>(TestUrl);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(GetCachedTestPokemon().Name));
    }
    
    private static Pokemon GetCachedTestPokemon()
    {
        return new Pokemon
        {
            Name = PokemonName
        };
    }
    
    private static string GetTestHttpResponse()
    {
        var testObject = GetCachedTestPokemon();
        return JsonSerializer.Serialize(testObject);
    }
}