using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PokemonLookup.Core.Services;
using PokemonLookup.Web;
using Testcontainers.PostgreSql;
using Xunit;

namespace PokemonLookup.LoadTests;

public sealed class TestingWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Launch the Web App in Production Mode
    private const string EnvironmentConfiguration = "Production";

    // Database connection
    private const string DatabaseUser = "loadUser";
    private const string DatabasePassword = "loadPassword";
    private const string DatabaseDb = "Load";
    private const string DatabaseServer = "localhost";
    private const ushort DatabasePort = 5432;

    private readonly PostgreSqlContainer _database;

    /// <summary>
    /// This Fixture is used to set up the application with all dependencies once for all tests.
    /// A Testcontainers Postgres database is used in combination with the remote Pokédex Api.
    /// </summary>
    public TestingWebAppFactory()
    {
        // Prepare the Testcontainers Postgres database
        _database = new PostgreSqlBuilder()
            .WithUsername(DatabaseUser)
            .WithPassword(DatabasePassword)
            .WithDatabase(DatabaseDb)
            .WithPortBinding(DatabasePort, DatabasePort)
            .Build();

        // Prepare the configuration of the app
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", EnvironmentConfiguration);
        Environment.SetEnvironmentVariable("DATABASE_USER", DatabaseUser);
        Environment.SetEnvironmentVariable("DATABASE_PASSWORD", DatabasePassword);
        Environment.SetEnvironmentVariable("DATABASE_DB", DatabaseDb);
        Environment.SetEnvironmentVariable("DATABASE_SERVER", DatabaseServer);
        Environment.SetEnvironmentVariable("DATABASE_PORT", DatabasePort.ToString());
    }

    /// <summary>
    /// Set up the application with all dependencies.
    /// This is executed once before the tests begin.
    /// </summary>
    public async Task InitializeAsync()
    {
        // Set up the postgres database
        await _database.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove services
            var dbContextDescriptor = services.Single(d =>
                d.ServiceType == typeof(IPokemonApiRequester)
            );

            services.Remove(dbContextDescriptor);

            // Add replacements
            services.AddSingleton<IPokemonApiRequester, DummyApiRequester>();
        });
    }

    /// <summary>
    /// Shut down the application and its dependencies.
    /// This is executed once after all tests completed.
    /// </summary>
    public override async ValueTask DisposeAsync()
    {
        // Shutdown the postgres database
        await _database.StopAsync();
        await _database.DisposeAsync();

        await base.DisposeAsync();
    }

    async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();
}
