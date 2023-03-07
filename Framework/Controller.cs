namespace Semifinals.Framework;

/// <summary>
/// Base class for controllers which manage requests made to the server through function triggers.
/// </summary>
/// <typeparam name="T">The service used to handle business logic</typeparam>
public abstract class Controller<T> where T : IService, new()
{
    public readonly T Service;

    public Controller()
    {
        Service = new T();
    }
}