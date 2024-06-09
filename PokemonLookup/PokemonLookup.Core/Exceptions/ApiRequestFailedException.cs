namespace PokemonLookup.Core.Exceptions;

public class ApiRequestFailedException : Exception
{
    public int ErrorCode { get; set; }

    public ApiRequestFailedException(Exception exception, int errorCode) : base($"Failed to request API. Error Code: {errorCode}", exception)
    {
        ErrorCode = errorCode;
    }

    public ApiRequestFailedException(Exception exception) : base($"Failed to request API. {exception.Message}", exception)
    {
        ErrorCode = -1;
    }
}