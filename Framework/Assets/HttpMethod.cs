namespace Semifinals.Framework;

/// <summary>
/// Enumerated HTTP methods that can be implemented.
/// </summary>
public enum HttpMethod
{
    /// <summary>
    /// Requests that fetch a resource or resources.
    /// </summary>
    GET,

    /// <summary>
    /// Requests that create a new resource.
    /// </summary>
    POST,

    /// <summary>
    /// Requests that create or update a resource by a given identifier.
    /// </summary>
    PUT,

    /// <summary>
    /// Deletes a resource.
    /// </summary>
    DELETE,

    /// <summary>
    /// Modifies the properties of a resource.
    /// </summary>
    PATCH
}