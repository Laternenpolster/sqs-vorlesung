using PokemonLookup.Core;

namespace PokemonLookup.UnitTests.Services;

/// <summary>
/// Check the user input filter.
/// </summary>
[TestFixture]
[TestOf(typeof(InputChecker))]
public class InputCheckerTest
{
    /// <summary>
    /// Input with letters and numbers are allowed.
    /// </summary>
    [Test]
    public void TestValidInputs()
    {
        var checker = new InputChecker();
        Assert.Multiple(() =>
        {
            Assert.That(checker.IsUserInputValid("a"), Is.True);
            Assert.That(checker.IsUserInputValid("A"), Is.True);
            Assert.That(checker.IsUserInputValid("1"), Is.True);
            Assert.That(checker.IsUserInputValid("ab0"), Is.True);
            Assert.That(checker.IsUserInputValid("test"), Is.True);
        });
    }

    /// <summary>
    /// Special characters are not allowed.
    /// </summary>
    [Test]
    public void TestInvalidInputs()
    {
        var checker = new InputChecker();
        Assert.Multiple(() =>
        {
            // Simple cases
            Assert.That(checker.IsUserInputValid(""), Is.False);
            Assert.That(checker.IsUserInputValid(" "), Is.False);
            Assert.That(checker.IsUserInputValid("."), Is.False);
            Assert.That(checker.IsUserInputValid("-"), Is.False);
            Assert.That(checker.IsUserInputValid(";"), Is.False);
            Assert.That(checker.IsUserInputValid("="), Is.False);
            Assert.That(checker.IsUserInputValid(":"), Is.False);
            Assert.That(checker.IsUserInputValid("/"), Is.False);
            Assert.That(checker.IsUserInputValid("\\"), Is.False);

            // Complex cases
            Assert.That(checker.IsUserInputValid("test name"), Is.False);
            Assert.That(checker.IsUserInputValid("test.name"), Is.False);
            Assert.That(checker.IsUserInputValid("test-name"), Is.False);
            Assert.That(checker.IsUserInputValid("test;name"), Is.False);
            Assert.That(checker.IsUserInputValid("test=name"), Is.False);
            Assert.That(checker.IsUserInputValid("test:name"), Is.False);
            Assert.That(checker.IsUserInputValid("test/name"), Is.False);
            Assert.That(checker.IsUserInputValid("test\\name"), Is.False);
            Assert.That(checker.IsUserInputValid(" testname"), Is.False);
            Assert.That(checker.IsUserInputValid("testname "), Is.False);
            Assert.That(checker.IsUserInputValid("test/../name"), Is.False);
            Assert.That(checker.IsUserInputValid("../name"), Is.False);
        });
    }
}
