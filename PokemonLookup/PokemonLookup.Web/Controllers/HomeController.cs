using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PokemonLookup.Web.Models;

namespace PokemonLookup.Web.Controllers;

/// <summary>
/// This controller displays the pages Home (Index) and Error.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// The Home page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Displayed when a page has an error.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var viewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View(viewModel);
    }
}
