using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace PokemonLookup.IntegrationTests;

public class TestingWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove services
            var dbContextDescriptor = services.Single(
                d => d.ServiceType ==
                     typeof(DbContextOptions<DbContext>));

            services.Remove(dbContextDescriptor);

            // Add replacements
            services.AddDbContext<DbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(TestingWebAppFactory));
            });
        });
    }
}