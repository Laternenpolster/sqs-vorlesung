using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PokemonLookup.End2EndTests;

/// <summary>
/// Checks the whole app from a user perspective through a browser.
/// The application is started with the same configuration it would have in production.
/// For more details see <see cref="WebServerFixture"/>.
/// </summary>
/// <param name="fixture">Used to set up the application dependencies</param>
public class FrontendTests(WebServerFixture fixture) : IClassFixture<WebServerFixture>
{
    /// <summary>
    /// Simulates a search for the Pokémon "Pikachu", which is a valid Pokémon.
    /// The user should see a page with details about the Pokémon.
    /// </summary>
    [Fact]
    public async Task TestValidRequest()
    {
        var page = await fixture.NewPageAsync();

        // Start the test on the Home page
        await page.GotoAsync("/");

        await Expect(page).ToHaveTitleAsync("Home Page - Pokémon Lookup");

        var searchBox = page.GetByPlaceholder("Enter a pokemon");
        var searchButton = page.Locator("[value=\"Search\"]");

        // Search for the existing Pokémon "Pikachu"
        const string pokemonName = "pikachu";
        await searchBox.FillAsync(pokemonName);
        await searchButton.ClickAsync();

        await Expect(page).ToHaveURLAsync($"/pokemon?name={pokemonName}");
        await Expect(page).ToHaveTitleAsync("Search result - Pokémon Lookup");

        // Check if the search result is correct
        var resultTitle = page.GetByRole(AriaRole.Heading);
        var pokemonId = page.Locator("#pokemonId");
        var pokemonWeight = page.Locator("#pokemonWeight");
        var pokemonHeight = page.Locator("#pokemonHeight");

        await Expect(resultTitle).ToHaveTextAsync(pokemonName);
        await Expect(pokemonId).ToHaveTextAsync("25");
        await Expect(pokemonWeight).ToHaveTextAsync("60");
        await Expect(pokemonHeight).ToHaveTextAsync("4");
    }

    /// <summary>
    /// Simulates a search for "unknown", which is not a valid Pokémon.
    /// The user should see an error page.
    /// </summary>
    [Fact]
    public async Task TestUnknownPokemon()
    {
        var page = await fixture.NewPageAsync();

        // Start the test on the Home page
        await page.GotoAsync("/");

        var searchBox = page.GetByPlaceholder("Enter a pokemon");
        var searchButton = page.Locator("[value=\"Search\"]");

        // Search for the unknown Pokémon "unknown"
        const string pokemonName = "unknown";
        await searchBox.FillAsync(pokemonName);
        await searchButton.ClickAsync();

        await Expect(page).ToHaveURLAsync($"/pokemon?name={pokemonName}");
        await Expect(page).ToHaveTitleAsync("Search result - Pokémon Lookup");

        // Check if the correct error message is displayed
        var resultTitle = page.GetByRole(AriaRole.Heading);
        var errorMessage = page.GetByText("Pokemon `unknown` was not found.");

        await Expect(resultTitle).ToHaveTextAsync("Error:");
        await Expect(errorMessage).ToBeVisibleAsync();
    }
}
