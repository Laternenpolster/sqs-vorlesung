using PokemonLookup.Application;

namespace PokemonLookup.UnitTests.Services;

/// <summary>
/// Check the user input filter.
/// </summary>
public class InputCheckerTest
{
    /// <summary>
    /// Input with letters and numbers are allowed.
    /// </summary>
    [Fact]
    public void TestValidInputs()
    {
        var checker = new InputChecker();

        Assert.True(checker.IsUserInputValid("a"));
        Assert.True(checker.IsUserInputValid("A"));
        Assert.True(checker.IsUserInputValid("1"));
        Assert.True(checker.IsUserInputValid("ab0"));
        Assert.True(checker.IsUserInputValid("test"));
    }

    /// <summary>
    /// Special characters are not allowed.
    /// </summary>
    [Fact]
    public void TestInvalidInputs()
    {
        var checker = new InputChecker();

        // Simple cases
        Assert.False(checker.IsUserInputValid(""));
        Assert.False(checker.IsUserInputValid(" "));
        Assert.False(checker.IsUserInputValid("."));
        Assert.False(checker.IsUserInputValid("-"));
        Assert.False(checker.IsUserInputValid(";"));
        Assert.False(checker.IsUserInputValid("="));
        Assert.False(checker.IsUserInputValid(":"));
        Assert.False(checker.IsUserInputValid("/"));
        Assert.False(checker.IsUserInputValid("\\"));

        // Complex cases
        Assert.False(checker.IsUserInputValid("test name"));
        Assert.False(checker.IsUserInputValid("test.name"));
        Assert.False(checker.IsUserInputValid("test-name"));
        Assert.False(checker.IsUserInputValid("test;name"));
        Assert.False(checker.IsUserInputValid("test=name"));
        Assert.False(checker.IsUserInputValid("test:name"));
        Assert.False(checker.IsUserInputValid("test/name"));
        Assert.False(checker.IsUserInputValid("test\\name"));
        Assert.False(checker.IsUserInputValid(" testname"));
        Assert.False(checker.IsUserInputValid("testname "));
        Assert.False(checker.IsUserInputValid("test/../name"));
        Assert.False(checker.IsUserInputValid("../name"));
    }
}
