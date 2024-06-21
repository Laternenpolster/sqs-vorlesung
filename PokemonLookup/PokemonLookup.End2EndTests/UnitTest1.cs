using static Microsoft.Playwright.Assertions;

namespace PokemonLookup.End2EndTests;

public class Tests(WebServerFixture fixture) : IClassFixture<WebServerFixture>
{
    [Fact]
    public async Task Example()
    {
        var page = await fixture.NewPageAsync();

        await page.GotoAsync("/");

        await Expect(page).ToHaveTitleAsync("Home Page - Pokémon Lookup");
    }
}
