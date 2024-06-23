namespace PokemonLookup.Application.Exceptions;

/// <summary>
/// A request to an API was not successful.
/// </summary>
public class ApiRequestFailedException : Exception
{
    /// <summary>
    /// The HTTP Error Code.
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// An HTTP request exception with an error code.
    /// </summary>
    /// <param name="exception">The inner exception</param>
    /// <param name="errorCode">The HTTP status code</param>
    public ApiRequestFailedException(Exception exception, int errorCode)
        : base($"Failed to request API. Error Code: {errorCode}", exception)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// An exception that occured during the API request, but does not have an error code.
    /// </summary>
    /// <param name="exception">The inner exception</param>
    public ApiRequestFailedException(Exception exception)
        : base($"Failed to request API. {exception.Message}", exception)
    {
        ErrorCode = -1;
    }
}
