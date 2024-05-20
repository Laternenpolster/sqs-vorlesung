using PokemonLookup.Core.Exceptions;
using PokemonLookup.Core.Services;

namespace PokemonLookup.Web.Services;

public class ApiRequester(HttpClient client) : IApiRequester
{
    public async Task<T> GetRequest<T>(string url) where T : class
    {
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<T>();

            return result!;
        }
        catch (HttpRequestException requestException)
        {
            throw new ApiRequestFailedException(requestException, (int)requestException.StatusCode!);
        }
        catch (Exception exception)
        {
            throw new ApiRequestFailedException(exception);
        }
    }
}