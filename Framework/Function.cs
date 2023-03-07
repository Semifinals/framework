using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Semifinals.Framework.Services.Jwt;

namespace Semifinals.Framework;

/// <summary>
/// A support class to handle HTTP requests.
/// </summary>
/// <typeparam name="T">The expected DTO of the request to validate</typeparam>
public class Function<T> where T : Dto
{
    public readonly bool RequiresAuth;
    public readonly int RequiresFlags;

    public readonly HttpMethod Method;
    public readonly IQueryCollection Query;
    public readonly T Body;
    public readonly IHeaderDictionary Headers;

    /// <summary>
    /// The authenticated user of the request if present
    /// </summary>
    public readonly IUser? User;

    public Function(Config config, HttpRequest req, T body, bool requiresAuth, int requiresFlags)
    {
        RequiresAuth = requiresAuth;
        RequiresFlags = requiresFlags;

        if (Enum.TryParse(req.Method.ToUpper(), out HttpMethod method))
            Method = method;
        else
            Method = HttpMethod.GET;

        Query = req.Query;
        Body = body;
        Headers = req.Headers;

        // Get user from authorization header if present
        if (Headers.TryGetValue("Authorization", out var authorizationHeaders))
        {
            string authorizationHeader = authorizationHeaders.First();

            if (authorizationHeader.StartsWith("Bearer "))
            {
                string jwt = authorizationHeader[7..];
                if (JwtService.TryParse<IUser>(jwt, config.JwtSecret, out var parsedJwt))
                    User = parsedJwt!.Payload.Content;
            }
        }
    }

    /// <summary>
    /// Handle HTTP trigger requests by validation the DTO and then progressing to a callback to handle controller logic.
    /// </summary>
    /// <param name="req">The HTTP request object</param>
    /// <param name="requiresAuth">Whether or not a valid authorization header needs to be present</param>
    /// <param name="requiresFlags">The user flags necessary on the authorized user to run the function</param>
    /// <returns></returns>
    public static Func<Func<Function<T>, Task<IActionResult>>, Task<IActionResult>> Run(
        Config config,
        HttpRequest req,
        bool requiresAuth = false,
        int requiresFlags = 0)
    {
        return new(async callback =>
        {
            // Read body of request
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            JObject body = JObject.Parse(requestBody);
            T parsedBody = body.ToObject<T>();

            // Validate DTO
            ValidationResult res = parsedBody.Validator.Test(parsedBody);
            if (!res.IsValid) return new BadRequestObjectResult(res.Errors.Select(err => err.ErrorMessage));

            // Create and run func
            Function<T> func = new(config, req, parsedBody, requiresAuth, requiresFlags);
            return await func.Run(callback);
        });
    }

    /// <summary>
    /// Handle requests that passed DTO validation by running a provided callback.
    /// </summary>
    /// <param name="func">The controller logic of the request</param>
    /// <returns>The result of the request's controller logic</returns>
    public async Task<IActionResult> Run(Func<Function<T>, Task<IActionResult>> func)
    {
        // Require a user exist if the function requires auth
        if (RequiresAuth && User == null)
            return new UnauthorizedResult();

        // Require that the user has all necessary user flags
        if (User != null && (User.Flags & RequiresFlags) != RequiresFlags)
            return new ForbidResult();

        // Allow passing function calls to run
        return await func(this);
    }
}