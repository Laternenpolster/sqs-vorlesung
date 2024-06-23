using Microsoft.Playwright;
using Testcontainers.PostgreSql;
using Program = PokemonLookup.Web.Program;

namespace PokemonLookup.End2EndTests;

/// <summary>
/// This Fixture is used to set up the application with all dependencies once for all tests.
/// The application is started with the same configuration it would have in production.
/// A Testcontainers Postgres database is used in combination with the remote Pokédex Api.
/// </summary>
public sealed class WebServerFixture : IAsyncLifetime, IAsyncDisposable
{
    // Launch the Web App in Production Mode
    private const string EnvironmentConfiguration = "Production";

    // Database connection
    private const string DatabaseUser = "e2eUser";
    private const string DatabasePassword = "e2ePassword";
    private const string DatabaseDb = "E2E";
    private const string DatabaseServer = "localhost";

    private readonly PostgreSqlContainer _database;
    private WebApplication? _webApplication;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    /// <summary>
    /// This Fixture is used to set up the application with all dependencies once for all tests.
    /// The application is started with the same configuration it would have in production.
    /// A Testcontainers Postgres database is used in combination with the remote Pokédex Api.
    /// </summary>
    public WebServerFixture()
    {
        // Prepare the Testcontainers Postgres database
        _database = new PostgreSqlBuilder()
            .WithUsername(DatabaseUser)
            .WithPassword(DatabasePassword)
            .WithDatabase(DatabaseDb)
            .Build();
    }

    /// <summary>
    /// Set up the application with all dependencies.
    /// This is executed once before the tests begin.
    /// </summary>
    public async Task InitializeAsync()
    {
        // Set up the postgres database
        await _database.StartAsync();
        var port = _database.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort);

        // Prepare the configuration of the app
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", EnvironmentConfiguration);
        Environment.SetEnvironmentVariable("DATABASE_USER", DatabaseUser);
        Environment.SetEnvironmentVariable("DATABASE_PASSWORD", DatabasePassword);
        Environment.SetEnvironmentVariable("DATABASE_DB", DatabaseDb);
        Environment.SetEnvironmentVariable("DATABASE_SERVER", DatabaseServer);
        Environment.SetEnvironmentVariable("DATABASE_PORT", port.ToString());

        // Run the ASP.NET Web App in the background
        Program.RunInBackground = true;
        await Program.Main([]);

        _webApplication = Program.App;

        // Set up playwright
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
    }

    /// <summary>
    /// Shut down the application and its dependencies.
    /// This is executed once after all tests completed.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        // Shutdown the app
        if (_webApplication != null)
        {
            await _webApplication.StopAsync();
            await _webApplication.DisposeAsync();
        }

        // Shutdown the postgres database
        await _database.StopAsync();
        await _database.DisposeAsync();

        // Shutdown Playwright
        if (_browser != null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();
    }

    async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();

    /// <summary>
    /// Creates a new page that is required for all tests.
    /// </summary>
    /// <returns>The page to test with</returns>
    public async Task<IPage> NewPageAsync()
    {
        var options = new BrowserNewPageOptions
        {
            // Copy the BaseURL from the application so that tests can use relative URLs.
            BaseURL = _webApplication!.Urls.First()
        };

        return await _browser!.NewPageAsync(options);
    }
}
