using PokemonLookup.Web.Controllers;

namespace PokemonLookup.Web.Models;

/// <summary>
/// Used to display an error from <see cref="HomeController"/>
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
