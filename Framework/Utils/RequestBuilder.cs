using Newtonsoft.Json;

namespace Semifinals.Framework.Utils;

/// <summary>
/// Builder class to make HTTP requests.
/// </summary>
public class RequestBuilder
{
    public readonly HttpMethod Method;

    public readonly string Url;

    public dynamic? Body;

    public Dictionary<string, string> Headers = new();

    public readonly Dictionary<int, Func<string, Task>> Handlers = new();

    public RequestBuilder(HttpMethod method, string url)
    {
        Method = method;
        Url = url;
    }

    /// <summary>
    /// Add a handler to a given status code.
    /// </summary>
    /// <param name="statusCode">The status code to handle</param>
    /// <param name="callback">The logic to handle the response</param>
    /// <returns>This builder</returns>
    public RequestBuilder Handle(int statusCode, Func<string, Task> callback)
    {
        Handlers.Add(statusCode, callback);
        return this;
    }

    /// <summary>
    /// Add a body to the request.
    /// </summary>
    /// <typeparam name="T">The type of the body</typeparam>
    /// <param name="body">The body to add</param>
    /// <returns>This builder</returns>
    public RequestBuilder AddBody<T>(T body) where T : Dto, IBodyDto
    {
        Body = body;
        return this;
    }

    /// <summary>
    /// Add a header to the request.
    /// </summary>
    /// <param name="name">The name of the header</param>
    /// <param name="value">The value of the header</param>
    /// <returns>This builder</returns>
    public RequestBuilder AddHeader(string name, string value)
    {
        Headers.Add(name, value);
        return this;
    }

    /// <summary>
    /// Make the request built from the builder.
    /// </summary>
    public async Task Call()
    {
        // Create and call web request
        using HttpClient client = new();

        foreach (var kvp in Headers)
            client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);

        string json = JsonConvert.SerializeObject(Body);
        StringContent httpContent = new(json, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage res = Method switch
        {
            HttpMethod.POST => await client.PostAsync(Url, httpContent),
            HttpMethod.PUT => await client.PutAsync(Url, httpContent),
            HttpMethod.PATCH => await client.PatchAsync(Url, httpContent),
            HttpMethod.DELETE => await client.DeleteAsync(Url),
            _ => await client.GetAsync(Url),
        };

        // Run handler
        int statusCode = (int)res.StatusCode;

        if (Handlers.ContainsKey(statusCode))
            await Handlers[statusCode](await res.Content.ReadAsStringAsync());
        else
            throw new NotHandledException(statusCode);
    }
}