using FluentValidation.Results;

namespace Semifinals.Framework;

/// <summary>
/// Represents errors that occur when attempting to validate an object.
/// </summary>
public class ValidationException : Exception
{
    public readonly List<ValidationFailure> Errors;

    public ValidationException(List<ValidationFailure> errors)
    {
        Errors = errors;
    }
}