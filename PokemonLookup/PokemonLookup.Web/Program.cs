using Microsoft.EntityFrameworkCore;
using PokemonLookup.Infrastructure;
using PokemonLookup.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();
builder.Services.AddInfrastructureServices();

// Build the connection string for the database
var dbUser = Environment.GetEnvironmentVariable("DATABASE_USER");
var dbPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
var dbDatabase = Environment.GetEnvironmentVariable("DATABASE_DB");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql($"Host=postgres;Username={dbUser};Password={dbPassword};Database={dbDatabase}"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Check if the required database structure exists
using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
await dbContext.Database.EnsureCreatedAsync();

await app.RunAsync();

// Needed for the IntegrationTest project to reference this project
public partial class Program;