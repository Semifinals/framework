namespace Semifinals.Framework.Services.Jwt;

/// <summary>
/// Base interface expected of a user to be used in authentication.
/// </summary>
public interface IUser
{
    int Flags { get; }
}