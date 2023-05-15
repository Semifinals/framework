using shortid;
using shortid.Configuration;

namespace Semifinals.Framework.Services.Id;

/// <summary>
/// Service to handle generating unique IDs.
/// </summary>
public static class IdService
{
    public static readonly string RouteConstraint = "regex([a-zA-Z0-9]{16})";
    
    public static readonly GenerationOptions Options = new(
        useNumbers: true,
        useSpecialCharacters: false,
        length: 16);

    /// <summary>
    /// Generate a unique ID using the standard options.
    /// </summary>
    /// <returns>A new unique ID</returns>
    public static string Generate()
    {
        return ShortId.Generate(Options);
    }
}