using Microsoft.EntityFrameworkCore;
using PokemonLookup.Infrastructure;
using PokemonLookup.Infrastructure.Data;

namespace PokemonLookup.Web;

// Needed for the IntegrationTest project to reference this project
#pragma warning disable
public class Program
#pragma warning restore
{
    public static bool RunInBackground { get; set; }
    public static WebApplication? App { get; private set; }

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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
