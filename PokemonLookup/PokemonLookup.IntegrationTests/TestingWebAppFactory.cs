using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PokemonLookup.Infrastructure.Data;
using PokemonLookup.Web;

namespace PokemonLookup.IntegrationTests;

/// <summary>
/// Set up the application with all dependencies once for all tests.
/// The Postgres database of the Production environment is replaced with an In Memory database.
/// </summary>
public class TestingWebAppFactory : WebApplicationFactory<Program>
{
    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Change the services used in Production
        builder.ConfigureServices(services =>
        {
            // Remove services
            var dbContextDescriptor = services.Single(d =>
                d.ServiceType == typeof(DbContextOptions<DataContext>)
            );

            services.Remove(dbContextDescriptor);

            // Add replacements
            services.AddDbContext<DataContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(TestingWebAppFactory));
            });
        });
    }
}
