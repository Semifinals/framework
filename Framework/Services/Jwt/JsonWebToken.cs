using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semifinals.Framework.Utils.Converters;
using System.Text;

namespace Semifinals.Framework.Services.Jwt;

public class JsonWebToken<T>
{
    public readonly JwtHeader Header;
    public readonly JwtPayload<T> Payload;

    public JsonWebToken(T content, string subject)
    {
        Header = new();
        Payload = new(content, subject);
    }

    public JsonWebToken(JwtPayload<T> payload)
    {
        Header = new();
        Payload = payload;
    }
}

public class JwtHeader
{
    [JsonProperty("alg")]
    public readonly string Algorithm = "HS256";

    [JsonProperty("typ")]
    public readonly string Type = "JWT";

    public override string ToString()
    {
        string headerUnencoded = JsonConvert.SerializeObject(this);
        string header = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerUnencoded));
        return header;
    }
}

public class JwtPayload<T>
{
    [JsonProperty("iat")]
    [JsonConverter(typeof(EpochConverter))]
    public readonly DateTime IssuedAt;

    [JsonProperty("exp")]
    [JsonConverter(typeof(EpochConverter))]
    public readonly DateTime Expires;

    [JsonProperty("sub")]
    public readonly string Subject;

    [JsonIgnore]
    public T Content { get; set; }

    public JwtPayload(T content, string subject)
    {
        Content = content;
        Subject = subject;
        IssuedAt = DateTime.UtcNow;
        Expires = DateTime.UtcNow.AddDays(7);
    }

    public override string ToString()
    {
        // Serialize base and content to strings
        string basePayload = JsonConvert.SerializeObject(this);
        string contentPayload = JsonConvert.SerializeObject(Content);

        // Convert strings to JObjects and merge
        JObject payloadObject = JObject.Parse(basePayload);
        JObject contentPayloadObject = JObject.Parse(contentPayload);

        payloadObject.Merge(contentPayloadObject, new()
        {
            MergeArrayHandling = MergeArrayHandling.Union
        });

        // Encode payload to base64
        string payloadUnencoded = payloadObject.ToString(Formatting.None);
        string payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadUnencoded));
        return payload;
    }
}