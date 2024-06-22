using System.Text.RegularExpressions;
using PokemonLookup.Application.Services;

namespace PokemonLookup.Application;

/// <inheritdoc/>
public partial class InputChecker : IInputChecker
{
    /// <inheritdoc/>
    public bool IsUserInputValid(string input)
    {
        // The input must only contain letters and numbers.
        return CharacterWhitelist().IsMatch(input);
    }

    [GeneratedRegex("^[a-zA-Z0-9]+$", RegexOptions.Compiled)]
    private static partial Regex CharacterWhitelist();
}
