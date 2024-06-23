using Microsoft.EntityFrameworkCore;
using PokemonLookup.Infrastructure;
using PokemonLookup.Infrastructure.Data;

namespace PokemonLookup.Web;

/// <summary>
/// The entrypoint of the ASP.NET Web App.
/// </summary>
#pragma warning disable
public class Program // Warning disabled, false positive. Needed for the IntegrationTest project to reference this project.
#pragma warning restore
{
    /// <summary>
    /// Configuration for E2E Tests
    /// </summary>
    public static bool RunInBackground { get; set; }
    public static WebApplication? App { get; private set; }

    /// <summary>
    /// The entrypoint of the ASP.NET Web App.
    /// </summary>
    /// <param name="args">Launch arguments</param>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Load correct controllers when started from E2E test
        builder.Services.AddMvc().AddApplicationPart(typeof(Program).Assembly);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddHttpClient();
        builder.Services.AddInfrastructureServices();

        // Build the connection string for the database
        var dbUser = Environment.GetEnvironmentVariable("DATABASE_USER");
        var dbPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
        var dbDatabase = Environment.GetEnvironmentVariable("DATABASE_DB");
        var dbServer = Environment.GetEnvironmentVariable("DATABASE_SERVER");
        var dbPort = Environment.GetEnvironmentVariable("DATABASE_PORT");

        builder.Services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(
                $"Host={dbServer};Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbDatabase}"
            )
        );

        App = builder.Build();

        // Configure the HTTP request pipeline.
        if (!App.Environment.IsDevelopment())
        {
            App.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            App.UseHsts();
        }

        App.UseHttpsRedirection();
        App.UseStaticFiles();

        App.UseRouting();

        App.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // Check if the required database structure exists
        using var scope = App.Services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // If the app is launched from E2E Tests, it needs to run in the background
        if (RunInBackground)
        {
            await App.StartAsync();
        }
        else
        {
            await App.RunAsync();
        }
    }
}
