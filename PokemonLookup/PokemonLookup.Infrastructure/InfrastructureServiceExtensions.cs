using Microsoft.Extensions.DependencyInjection;
using PokemonLookup.Core;
using PokemonLookup.Core.Services;
using PokemonLookup.Infrastructure.ExternalLookup;

namespace PokemonLookup.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IInputChecker, InputChecker>();
        services.AddScoped<ICachingService, CachingService>();
        services.AddSingleton<IApiRequester, ApiRequester>();
        services.AddSingleton<IPokemonApiRequester, PokemonApiRequester>();
        services.AddScoped<IPokemonLibrary, PokemonLibrary>();

        return services;
    }
}