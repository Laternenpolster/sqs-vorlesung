using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;

namespace PokemonLookup.LoadTests;

/// <summary>
/// Checks the performance of the whole app during high load.
/// Postgres is used like in Production, the Pokédex API on the other hand is not used.
/// </summary>
/// <param name="factory">Used to set up the application dependencies</param>
public class Test(TestingWebAppFactory factory)
    : IClassFixture<TestingWebAppFactory>
{
    // The test process is identical for every test
    private static readonly LoadSimulation[] LoadSimulations =
    [
        Simulation.RampingInject(rate: 500,
            interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromSeconds(30)),
        Simulation.Inject(500,
            interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromSeconds(60)
        )
    ];

    /// <summary>
    /// Tests the performance of the Pokémon Details Page and the builtin Pokémon API.
    /// These are the only interesting targets, as no other pages depend on the database or upstream API.
    /// The application has ~30 users, but the goal is to be able to handle 500 requests per second.
    /// </summary>
    [Fact]
    public async Task TestPerformance()
    {
        var httpClient = factory.CreateClient();

        // Check that the URL is reachable
        var webCheck = await httpClient.GetAsync("/Pokemon?name=pikachu");
        var apiCheck = await httpClient.GetAsync("/api/v1/pokemon/ditto");
        webCheck.EnsureSuccessStatusCode();
        apiCheck.EnsureSuccessStatusCode();

        // Define the tests
        var webScenario = Scenario.Create("website", async _ =>
            {
                var request = Http
                    .CreateRequest("GET", "/Pokemon?name=pikachu")
                    .WithHeader("Accept", "text/html");

                var response = await Http.Send(httpClient, request);

                return response;
            })
            .WithLoadSimulations(LoadSimulations);

        var apiScenario = Scenario.Create("api", async _ =>
            {
                var request = Http
                    .CreateRequest("GET", "/api/v1/pokemon/ditto")
                    .WithHeader("Accept", "text/html");

                var response = await Http.Send(httpClient, request);

                return response;
            })
            .WithLoadSimulations(LoadSimulations);

        // Execute the tests
        var result = NBomberRunner
            .RegisterScenarios(webScenario, apiScenario)
            .Run();

        var webStats = result.ScenarioStats.Get("website");
        var apiStats = result.ScenarioStats.Get("api");

        // Assertions
        Assert.Equal(0, webStats.AllFailCount);
        Assert.Equal(0, apiStats.AllFailCount);

        Assert.True(webStats.Ok.Latency.Percent75 < 30);
        Assert.True(webStats.Ok.Latency.Percent99 < 75);

        Assert.True(apiStats.Ok.Latency.Percent75 < 25);
        Assert.True(apiStats.Ok.Latency.Percent99 < 60);
    }
}
