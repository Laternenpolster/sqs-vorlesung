using System.Text.RegularExpressions;
using PokemonLookup.Core.Services;

namespace PokemonLookup.Core;

public partial class InputChecker : IInputChecker
{
    public bool IsUserInputValid(string input)
    {
        return CharacterWhitelist().IsMatch(input);
    }

    [GeneratedRegex("^[a-zA-Z0-9]+$", RegexOptions.Compiled)]
    private static partial Regex CharacterWhitelist();
}
