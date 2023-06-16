using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semifinals.Framework.Utils.Exceptions;
using Semifinals.Utils.Tokens;

namespace Semifinals.Framework;

/// <summary>
/// A support class to handle HTTP requests.
/// </summary>
/// <typeparam name="T1">The expected DTO of the request body</typeparam>
/// <typeparam name="T2">The expected DTO of the request params</typeparam>
public class Function<T1, T2>
    where T1 : Dto, IBodyDto
    where T2 : Dto, IParamDto
{
    /// <summary>
    /// Determines whether or not a user must be authenticated to call the function.
    /// </summary>
    public readonly bool RequiresAuth;

    /// <summary>
    /// The authenticated user of the request if present.
    /// </summary>
    public readonly string? User;

    /// <summary>
    /// The HTTP method of the request.
    /// </summary>
    public readonly HttpMethod Method;

    /// <summary>
    /// The HTTP headers of the request.
    /// </summary>
    public readonly IHeaderDictionary Headers;

    /// <summary>
    /// The parsed HTTP request body.
    /// </summary>
    public readonly T1 Body;

    /// <summary>
    /// The parsed HTTP request params.
    /// </summary>
    public readonly T2 Params;

    public Function(
        HttpRequest req,
        T1 body,
        T2 parameters,
        bool requiresAuth)
    {
        // Assign auth and flags
        RequiresAuth = requiresAuth;

        // Assign HTTP request properties to function
        if (Enum.TryParse(req.Method.ToUpper(), out HttpMethod method))
            Method = method;
        else
            Method = HttpMethod.GET;

        Headers = req.Headers;
        Body = body;
        Params = parameters;

        // Get user from authorization header if present
        if (Headers.TryGetValue("Authorization", out var authorizationHeaders))
        {
            string authorizationHeader = authorizationHeaders.First();

            if (authorizationHeader.StartsWith("Bearer "))
            {
                User = authorizationHeader[7..];
            }
        }
    }

    /// <summary>
    /// Validate the body of a HTTP request.
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <returns>The parsed body</returns>
    /// <exception cref="ValidationException">Occurs when validation fails</exception>
    public static async Task<T1> ValidateBody(HttpRequest req)
    {
        string requestBody;
        try
        {
            // Read body of request
            requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (requestBody == "null")
                throw new Exception();
        }
        catch (Exception)
        {
            throw new ArgumentNullException(nameof(req));
        }

        T1 parsedBody;
        try {
            JObject body = JsonConvert.DeserializeObject<JObject>(requestBody, new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            })!;

            parsedBody = body.ToObject<T1>()!;
        }
        catch (Exception)
        {
            throw new ArgumentException();
        }

        // Validate DTO
        ValidationResult res = parsedBody.Validator.Test(parsedBody);
        if (!res.IsValid)
            throw new ValidationException(res.Errors);

        return parsedBody;
    }

    /// <summary>
    /// Validate the request params of a HTTP request.
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <returns>The parsed params</returns>
    /// <exception cref="ValidationException">Occurs when validation fails</exception>
    public static T2 ValidateParams(HttpRequest req)
    {
        // Read params of request
        Dictionary<string, string> requestParams = new();

        foreach (var value in req.Query)
        {
            requestParams.Add(value.Key, value.Value.First()); // TODO: Check how this interacts with no parameters present
        }

        T2 parsedParams = JObject.FromObject(requestParams).ToObject<T2>()!; // TODO: Ensure property types are converted

        // Validate DTO
        ValidationResult res = parsedParams.Validator.Test(parsedParams);
        if (!res.IsValid) throw new ValidationException(res.Errors);

        return parsedParams;
    }

    /// <summary>
    /// Verify that the request authentication matches requirements for the function.
    /// </summary>
    /// <param name="func">The function to compare</param>
    /// <exception cref="UnauthorizedException">Occurs when the function requires a user to be authenticated but there isn't one</exception>
    /// <exception cref="ForbiddenException">Occurs when the function requires some permissions the user doesn't have</exception>
    public static void VerifyUser(Function<T1, T2> func)
    {
        // Require a user exist if the function requires auth
        if (func.RequiresAuth && func.User == null)
            throw new UnauthorizedException();
    }

    /// <summary>
    /// Run the validation and verification, and run the function if valid.
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <param name="requiresAuth">Whether or not an authorised user is required</param>
    /// <param name="requiresFlags">The permission flags required to use this function</param>
    /// <returns>A callback function to call with the function accessible</returns>
    public static Func<Func<Function<T1, T2>, Task<IActionResult>>, Task<IActionResult>> Run(
        HttpRequest req,
        bool requiresAuth = false)
    {
        return new(async callback =>
        {
            // Validate the request body
            T1? parsedBody;

            try
            {
                if (typeof(T1) == typeof(NoBodyDto))
                    parsedBody = new NoBodyDto() as T1;
                else
                    parsedBody = await ValidateBody(req);
            }
            catch (ValidationException ex)
            {
                return new BadRequestObjectResult(
                    ex.Errors.Select(err => ValidationErrorResult.FromFailure(err)));
            }
            catch (ArgumentNullException ex)
            {
                return new BadRequestObjectResult(
                    new ValidationErrorResult[] { new("F000", ex.Message) });
            }
            catch (ArgumentException)
            {
                return new BadRequestObjectResult(
                    new ValidationErrorResult[] { new("F001", "Validation passed on an invalid body") });
            }

            // Validate the request params
            T2 parsedParams;

            try
            {
                parsedParams = ValidateParams(req);
            }
            catch (ValidationException ex)
            {
                return new BadRequestObjectResult(
                    ex.Errors.Select(err => ValidationErrorResult.FromFailure(err)));
            }

            // Verify the user
            Function<T1, T2> func = new(
                req,
                parsedBody!,
                parsedParams,
                requiresAuth);

            try
            {
                VerifyUser(func);
            }
            catch (UnauthorizedException)
            {
                return new UnauthorizedResult();
            }
            catch (ForbiddenException)
            {
                return new ForbidResult();
            }

            // Run the function on valid request
            return await callback(func);
        });
    }
}

/// <summary>
/// A support class to handle HTTP requests.
/// </summary>
/// <typeparam name="T1">The expected DTO of the request body</typeparam>
public class Function<T1> : Function<T1, NoParamDto>
    where T1 : Dto, IBodyDto
{
    public Function(
        HttpRequest req,
        T1 body,
        bool requiresAuth)
    : base(
        req,
        body,
        new NoParamDto(),
        requiresAuth)
    { }
}

/// <summary>
/// A support class to handle HTTP requests.
/// </summary>
public class Function : Function<NoBodyDto, NoParamDto>
{
    public Function(
        HttpRequest req,
        bool requiresAuth)
    : base(
        req,
        new NoBodyDto(),
        new NoParamDto(),
        requiresAuth)
    { }
}