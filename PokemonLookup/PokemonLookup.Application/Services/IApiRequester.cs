using PokemonLookup.Application.Exceptions;

namespace PokemonLookup.Application.Services;

/// <summary>
/// Used to communicate with any REST APIs.
/// </summary>
public interface IApiRequester
{
    /// <summary>
    /// Sends a GET request to any API.
    /// An HTTP 200 response is expected and the result is deserialized from JSON.
    /// Otherwise <see cref="ApiRequestFailedException"/> is thrown.
    /// </summary>
    /// <param name="url">Where to send the GET request</param>
    /// <typeparam name="T">The deserialized type</typeparam>
    /// <returns>The deserialized result</returns>
    Task<T> GetRequest<T>(string url) where T : class;
}
