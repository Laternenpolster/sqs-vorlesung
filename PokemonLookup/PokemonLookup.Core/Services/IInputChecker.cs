namespace PokemonLookup.Core.Services;

/// <summary>
/// Used to check user input before it is handed to another service.
/// </summary>
public interface IInputChecker
{
    /// <summary>
    /// Check if the user input is a valid Pokémon name, before the text is handed to e.g. the database.
    /// </summary>
    /// <param name="input">The user input</param>
    /// <returns>true if the input is valid</returns>
    bool IsUserInputValid(string input);
}
