namespace Semifinals.Framework;

/// <summary>
/// Config necessary to use the framework's functions.
/// </summary>
public class Config
{
    public readonly string JwtSecret;
    
    public Config(string jwtSecret)
    {
        JwtSecret = jwtSecret;
    }
}