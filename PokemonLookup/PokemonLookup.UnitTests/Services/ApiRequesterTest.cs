using System.Net;
using System.Text.Json;
using PokemonLookup.Core.Entities;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Infrastructure.ExternalLookup;
using RichardSzalay.MockHttp;
using static PokemonLookup.UnitTests.TestDataProvider;

namespace PokemonLookup.UnitTests.Services;

/// <summary>
/// Test the generic REST API requester.
/// </summary>
public class ApiRequesterTest
{
    private const string TestUrl = "https://google.com";
    private const string ContentType = "application/json";

    /// <summary>
    /// Simulate a request to an existing endpoint.
    /// </summary>
    [Fact]
    public async Task TestValidRequest()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(TestUrl)
            .Respond(ContentType, GetValidHttpResponse());

        var httpClient = mockHttp.ToHttpClient();

        var apiRequester = new ApiRequester(httpClient);

        // Act
        var result = await apiRequester.GetRequest<Pokemon>(TestUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(GetValidTestPokemon().Name, result.Name);
    }

    /// <summary>
    /// Test the reaction to a 404 status code.
    /// The service should throw an <see cref="ApiRequestFailedException"/> with a status code.
    /// </summary>
    [Fact]
    public async Task TestNotFoundException()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(TestUrl)
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
            Assert.Equal(404, exception.ErrorCode);
        }
    }

    /// <summary>
    /// Simulate an error in the JSON deserialization.
    /// The service should throw an <see cref="ApiRequestFailedException"/> without a status code.
    /// </summary>
    [Fact]
    public async Task TestGenericException()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(TestUrl)
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
            Assert.Equal(-1, exception.ErrorCode);
        }
    }

    private static string GetValidHttpResponse()
    {
        var testObject = GetValidTestPokemon();
        return JsonSerializer.Serialize(testObject);
    }
}
