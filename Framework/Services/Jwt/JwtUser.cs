using Newtonsoft.Json;

namespace Semifinals.Framework.Services.Jwt;

/// <summary>
/// Base user to parse JWTs into.
/// </summary>
public class JwtUser : IUser
{
    [JsonProperty("flags")]
    public int Flags { get; }
}