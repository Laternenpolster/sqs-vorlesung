namespace PokemonLookup.Core.Exceptions;

/// <summary>
/// The user input did not meet the expectations or was blocked by the filter.
/// </summary>
/// <param name="msg">User Input</param>
public class InvalidUserInputException(string msg) : Exception(msg);
