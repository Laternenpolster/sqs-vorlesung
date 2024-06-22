using System.Net.Http.Json;
using PokemonLookup.Core.Exceptions;
using PokemonLookup.Core.Services;

namespace PokemonLookup.Infrastructure.ExternalLookup;

/// <inheritdoc/>
public class ApiRequester(HttpClient client) : IApiRequester
{
    /// <inheritdoc/>
    public async Task<T> GetRequest<T>(string url)
        where T : class
    {
        try
        {
            // Send the request and check for a 200 status code
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Deserialize the response body
            var result = await response.Content.ReadFromJsonAsync<T>();

            return result!;
        }
        // The response contained an unexpected status code
        catch (HttpRequestException requestException)
        {
            throw new ApiRequestFailedException(requestException, (int)requestException.StatusCode!);
        }
        // Wrap any other exception
        catch (Exception exception)
        {
            throw new ApiRequestFailedException(exception);
        }
    }
}
