using FluentValidation;

namespace Semifinals.Framework.Utils.Exceptions;

/// <summary>
/// Extension methods to set errors on validation.
/// </summary>
public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T1, T2> SetError<T1, T2>(
        this IRuleBuilderOptions<T1, T2> builder,
        string code,
        string message)
    {
        return builder.WithErrorCode(code).WithMessage(message);
    }

    public static IRuleBuilderOptions<T1, T2> SetError<T1, T2>(
        this IRuleBuilderOptions<T1, T2> builder,
        ValidationErrorResult error)
    {
        return builder.WithErrorCode(error.Code).WithMessage(error.Message);
    }
}
