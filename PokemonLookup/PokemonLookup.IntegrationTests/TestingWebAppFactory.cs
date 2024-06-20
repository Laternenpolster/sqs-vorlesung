using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PokemonLookup.Infrastructure.Data;
using PokemonLookup.Web;

namespace PokemonLookup.IntegrationTests;

public class TestingWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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
