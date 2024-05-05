using System.Text.Json;

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

            if (result == null)
            {
                throw new JsonException("Deserialized object is null!");
            }

            return result;
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