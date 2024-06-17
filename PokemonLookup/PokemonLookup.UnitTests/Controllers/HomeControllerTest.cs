using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonLookup.Web.Controllers;
using PokemonLookup.Web.Models;

namespace PokemonLookup.UnitTests.Controllers;

[TestFixture]
[TestOf(typeof(HomeController))]
public class HomeControllerTest
{
    [Test]
    public void TestHomePage()
    {
        // Arrange
        var controller = new HomeController();

        // Act
        var result = controller.Index();

        // Assert
        Assert.That(result, Is.TypeOf<ViewResult>());
    }

    [Test]
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
        Assert.That(result, Is.TypeOf<ViewResult>());

        var viewResult = (ViewResult)result;
        var model = (ErrorViewModel)viewResult.ViewData.Model!;
        Assert.Multiple(() =>
        {
            Assert.That(model.RequestId, Is.Not.Null);
            Assert.That(model.ShowRequestId, Is.True);
        });
    }
}
