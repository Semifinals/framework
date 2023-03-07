using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace Semifinals.Framework;

/// <summary>
/// Base class for data transfer objects used to represent HTTP request bodies.
/// </summary>
public abstract class Dto
{
    [JsonIgnore]
    public abstract IDtoValidator Validator { get; }
}

/// <summary>
/// Validator interface to handle generic DTO validators.
/// </summary>
public interface IDtoValidator
{
    /// <summary>
    /// Tests if a given request body matches the validator's DTO.
    /// </summary>
    /// <param name="body">The HTTP request body to test</param>
    /// <returns>A ValidationResult representing the validity of the body</returns>
    public ValidationResult Test(dynamic body);
}

/// <summary>
/// Validator used to check if an incoming request body matches the necessary DTO.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DtoValidator<T> : AbstractValidator<T>, IDtoValidator where T : Dto
{
    public DtoValidator(Action<DtoValidator<T>> callback)
    {
        callback(this);
    }

    public ValidationResult Test(dynamic body)
    {
        return Validate(body);
    }
}