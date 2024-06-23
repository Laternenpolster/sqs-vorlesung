using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonLookup.Web.Controllers;
using PokemonLookup.Web.Models;

namespace PokemonLookup.UnitTests.Controllers;

public class HomeControllerTest
{
    /// <summary>
    /// Tests that the Home Page is displayed as a View.
    /// </summary>
    [Fact]
    public void TestHomePage()
    {
        // Arrange
        var controller = new HomeController();

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    /// <summary>
    /// Test that the error page is displayed as a View with all information.
    /// </summary>
    [Fact]
    public void TestErrorPage()
    {
        // Arrange
        var controller = new HomeController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = controller.Error();

        // Assert
        Assert.IsType<ViewResult>(result);

        var viewResult = (ViewResult)result;
        var model = (ErrorViewModel)viewResult.ViewData.Model!;

        Assert.NotNull(model.RequestId);
        Assert.True(model.ShowRequestId);
    }
}
