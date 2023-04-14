namespace Semifinals.Framework;

/// <summary>
/// Represents errors that occur when a HTTP response is not handled.
/// </summary>
public class NotHandledException : Exception
{
    public readonly int StatusCode;

    public NotHandledException(int statusCode)
    {
        StatusCode = statusCode;
    }
}