using Microsoft.Playwright;
using Testcontainers.PostgreSql;
using Program = PokemonLookup.Web.Program;

namespace PokemonLookup.End2EndTests;

public class WebServerFixture : IAsyncLifetime, IAsyncDisposable
{
    private const string EnvironmentConfiguration = "Production";

    private const string DatabaseUser = "e2eUser";
    private const string DatabasePassword = "e2ePassword";
    private const string DatabaseDb = "E2E";
    private const string DatabaseServer = "localhost";

    private readonly PostgreSqlContainer _database;
    private WebApplication? _webApplication;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public WebServerFixture()
    {
        _database = new PostgreSqlBuilder()
            .WithUsername(DatabaseUser)
            .WithPassword(DatabasePassword)
            .WithDatabase(DatabaseDb)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        var port = _database.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort);

        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", EnvironmentConfiguration);
        Environment.SetEnvironmentVariable("DATABASE_USER", DatabaseUser);
        Environment.SetEnvironmentVariable("DATABASE_PASSWORD", DatabasePassword);
        Environment.SetEnvironmentVariable("DATABASE_DB", DatabaseDb);
        Environment.SetEnvironmentVariable("DATABASE_SERVER", DatabaseServer);
        Environment.SetEnvironmentVariable("DATABASE_PORT", port.ToString());

        Program.RunInBackground = true;
        await Program.Main([]);

        _webApplication = Program.App;

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_webApplication != null)
        {
            await _webApplication.StopAsync();
            await _webApplication.DisposeAsync();
        }

        await _database.StopAsync();
        await _database.DisposeAsync();

        if (_browser != null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();
    }

    async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();

    public async Task<IPage> NewPageAsync()
    {
        return await _browser!.NewPageAsync();
    }
}
