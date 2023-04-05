using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Semifinals.Framework;

/// <summary>
/// Builder class to create HTTP functions.
/// </summary>
/// <typeparam name="T1">The type of the body</typeparam>
/// <typeparam name="T2">The type of the request params</typeparam>
public class FunctionBuilder<T1, T2>
    where T1 : Dto, IBodyDto
    where T2 : Dto, IParamDto
{
    /// <summary>
    /// Add a body to the request.
    /// </summary>
    /// <typeparam name="T">The new type for the body</typeparam>
    /// <returns>The FunctionBuilder with the new body</returns>
    public FunctionBuilder<T, T2> AddBody<T>() where T : Dto, IBodyDto
    {
        return new();
    }

    /// <summary>
    /// Add request params to the request.
    /// </summary>
    /// <typeparam name="T">The new type for the request params</typeparam>
    /// <returns>The FunctionBuilder with the new request params</returns>
    public FunctionBuilder<T1, T> AddParams<T>() where T : Dto, IParamDto
    {
        return new();
    }

    /// <summary>
    /// Build the function ready to be handled with a callback.
    /// </summary>
    /// <param name="req">The HTTP request for the trigger</param>
    /// <param name="requiresAuth">Whether or not authentication is required</param>
    /// <param name="requiresFlags">The user permissions necessary for the function</param>
    /// <param name="tokenSecret">The secret used to encrypt tokens</param>
    /// <returns>A function that can be called with a callback to handle requests</returns>
    public Func<Func<Function<T1, T2>, Task<IActionResult>>, Task<IActionResult>> Build(
        HttpRequest req,
        bool requiresAuth = false,
        int requiresFlags = 0,
        string tokenSecret = "")
    {
        return Function<T1, T2>.Run(req, requiresAuth, requiresFlags, tokenSecret);
    }
}

/// <summary>
/// Builder class to create HTTP functions.
/// </summary>
public class FunctionBuilder
{
    /// <summary>
    /// Initialize a FunctionBuilder.
    /// </summary>
    /// <returns>A FunctionBuilder with no set body or request params</returns>
    public static FunctionBuilder<NoBodyDto, NoParamDto> Init()
    {
        return new();
    }
}