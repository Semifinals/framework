﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace Semifinals.Framework.Testing;

/// <summary>
/// Base class for tests.
/// </summary>
public class Test
{
    /// <summary>
    /// Create a HTTP request to be used to run a controller's function.
    /// </summary>
    /// <typeparam name="T">The type of the DTO to send</typeparam>
    /// <param name="dto">The DTO to send</param>
    /// <param name="method">The HTTP method to use in the request</param>
    /// <param name="authorizationHeader">The authorization header of the request</param>
    /// <param name="query">The query parameters of the request</param>
    /// <returns>The HTTP Request to send</returns>
    public static async Task<HttpRequest> CreateRequest<T>(
            T dto,
            HttpMethod method = HttpMethod.GET,
            string? authorizationHeader = null,
            QueryBuilder? query = null)
        where T : Dto
    {
        // Create http context for the request
        HttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.ContentType = "application/json";

        // Add body to request
        string body = JsonConvert.SerializeObject(dto);

        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(body);
        await writer.FlushAsync();
        stream.Position = 0;
        httpContext.Request.Body = stream;

        // Assign optional properties
        httpContext.Request.Method = method.ToString();

        if (authorizationHeader != null)
            httpContext.Request.Headers["Authorization"] = authorizationHeader;

        if (query != null)
            httpContext.Request.QueryString = query.ToQueryString();

        // Return http request
        return httpContext.Request;
    }

    /// <summary>
    /// Create a HTTP request to be used to run a controller's function.
    /// </summary>
    /// <param name="method">The HTTP method to use in the request</param>
    /// <param name="authorizationHeader">The authorization header of the request</param>
    /// <param name="query">The query parameters of the request</param>
    /// <returns>The HTTP Request to send</returns>
    public static async Task<HttpRequest> CreateRequest(
        HttpMethod method = HttpMethod.GET,
        string? authorizationHeader = null,
        QueryBuilder? query = null)
    {
        return await CreateRequest<NoBodyDto>(new(), method, authorizationHeader, query);
    }

    /// <summary>
    /// Generate a random string.
    /// </summary>
    /// <param name="minLength">The minimum length of the string</param>
    /// <param name="maxLength">The maximum length of the string</param>
    /// <returns>A random string</returns>
    public static string GenerateRandomString(int minLength = 16, int maxLength = 16)
    {
        Random rand = new();

        int length = rand.Next(minLength, maxLength);

        char[] chars = new char[length];
        string res = string.Join("", chars.Select(c => Convert.ToChar(65 + rand.Next(0, 26))));

        return res;
    }
}