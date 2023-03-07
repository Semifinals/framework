using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Semifinals.Framework.Services.Jwt;

/// <summary>
/// Service to handle Json Web Token operations.
/// </summary>
public class JwtService
{
    /// <summary>
    /// Check that a string looks like a JWT
    /// </summary>
    /// <param name="jwt">The string to test</param>
    /// <returns>Whether or not the JWT is valid</returns>
    public static bool Validate(string jwt)
    {
        string[] parts = jwt.Split('.');

        // Check the JWT contains 3 parts separated by dots
        if (parts.Length != 3)
            return false;

        // Check the header is valid
        string validHeader = JsonConvert.SerializeObject(new JwtHeader());
        byte[] validHeaderBytes = Encoding.UTF8.GetBytes(validHeader);
        string validHeaderBase64 = Convert.ToBase64String(validHeaderBytes);

        if (parts[0] != validHeaderBase64)
            return false;

        return true;
    }

    /// <summary>
    /// Verify that the JWT is authentic.
    /// </summary>
    /// <param name="jwt">The string to test</param>
    /// <param name="jwtSecret">The secret used to hash the JWT signature</param>
    /// <returns>Whether or not the JWT is authentic</returns>
    public static bool Verify(string jwt, string jwtSecret)
    {
        if (!Validate(jwt))
            return false;

        string[] parts = jwt.Split('.');

        // Check the signature matches the payload
        string signature = GenerateSignature(parts[0], parts[1], jwtSecret);
        if (signature != parts[2])
            return false;

        return true;
    }

    /// <summary>
    /// Try to parse a JWT string into a valid JWT object.
    /// </summary>
    /// <typeparam name="T">The type of the payload content</typeparam>
    /// <param name="jwt">The JWT string</param>
    /// <param name="jwtSecret">The secret used to hash the JWT signature</param>
    /// <param name="parsedJwt">The resulting JWT object</param>
    /// <returns>A bool representing if the parse was successful</returns>
    public static bool TryParse<T>(string jwt, string jwtSecret, out JsonWebToken<T>? parsedJwt)
    {
        parsedJwt = null;

        if (!Validate(jwt))
            return false;

        if (!Verify(jwt, jwtSecret))
            return false;

        string utf8Payload = Encoding.UTF8.GetString(Convert.FromBase64String(jwt.Split('.')[1]));

        T content = JsonConvert.DeserializeObject<T>(utf8Payload);
        JwtPayload<T> payload = JsonConvert.DeserializeObject<JwtPayload<T>>(utf8Payload);
        payload.Content = content;

        parsedJwt = new JsonWebToken<T>(payload);
        return true;
    }

    /// <summary>
    /// Generate the signature for a JWT object.
    /// </summary>
    /// <typeparam name="T">The type of the JWT payload</typeparam>
    /// <param name="jwt">The JWT to generate a signature for</param>
    /// <param name="jwtSecret">The secret used to hash the JWT signature</param>
    /// <returns>The signature for the given JWT</returns>
    public static string GenerateSignature<T>(JsonWebToken<T> jwt, string jwtSecret)
    {
        return GenerateSignature(jwt.Header.ToString(), jwt.Payload.ToString(), jwtSecret);
    }

    /// <summary>
    /// Generate the signature using header and payload strings.
    /// </summary>
    /// <param name="header">The JWT header string</param>
    /// <param name="payload">The JWT payload string</param>
    /// <param name="jwtSecret">The secret used to hash the JWT signature</param>
    /// <returns>The signature for the given JWT</returns>
    public static string GenerateSignature(string header, string payload, string jwtSecret)
    {
        // Format data to injest
        byte[] hashBytes = Encoding.UTF8.GetBytes($"{header}.{payload}");

        // Get hashing secret from env var
        byte[] secretBytes = Encoding.UTF8.GetBytes(jwtSecret);

        // Compute hash
        using var hasher = new HMACSHA256(secretBytes);
        byte[] signatureBytes = hasher.ComputeHash(hashBytes);

        // Return signature
        string signature = Convert.ToBase64String(signatureBytes);
        return signature;
    }

    /// <summary>
    /// Generate a JWT string from an object.
    /// </summary>
    /// <typeparam name="T">The type of the JWT payload</typeparam>
    /// <param name="jwt">The JWT to generate from</param>
    /// <param name="jwtSecret">The secret used to hash the JWT signature</param>
    /// <returns>A string representation of a JWT</returns>
    public static string Generate<T>(JsonWebToken<T> jwt, string jwtSecret)
    {
        string header = jwt.Header.ToString();
        string payload = jwt.Payload.ToString();
        string signature = GenerateSignature(jwt, jwtSecret);
        return $"{header}.{payload}.{signature}";
    }
}