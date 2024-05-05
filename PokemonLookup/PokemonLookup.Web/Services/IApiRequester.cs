namespace WebApplication1.Services;

public interface IApiRequester
{
    Task<T> GetRequest<T>(string url) where T : class;
}