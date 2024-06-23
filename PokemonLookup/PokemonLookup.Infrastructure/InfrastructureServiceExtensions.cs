using Microsoft.Extensions.DependencyInjection;
using PokemonLookup.Application;
using PokemonLookup.Application.Services;
using PokemonLookup.Infrastructure.Caching;
using PokemonLookup.Infrastructure.ExternalLookup;

namespace PokemonLookup.Infrastructure;

/// <summary>
/// Initialize the application with all required dependencies.
/// This can not only be used for ASP.NET Web Apps
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Initialize the application with all required dependencies.
    /// This can not only be used for ASP.NET Web Apps
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IInputChecker, InputChecker>();
        services.AddScoped<ICachingService, DatabaseCachingService>();
        services.AddSingleton<IApiRequester, ApiRequester>();
        services.AddSingleton<IPokemonApiRequester, PokemonApiRequester>();
        services.AddScoped<IPokemonLibrary, PokemonLibrary>();

        return services;
    }
}
