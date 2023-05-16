using FluentValidation.Results;
using Newtonsoft.Json;

namespace Semifinals.Framework.Utils.Exceptions;

/// <summary>
/// The result of a validation error.
/// </summary>
public class ValidationErrorResult
{
    [JsonProperty("code")]
    public string Code { get; }

    [JsonProperty("message")]
    public string Message { get; }

    public ValidationErrorResult(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Get an error result from the validation failure.
    /// </summary>
    /// <param name="failure">The validation failure</param>
    /// <returns>The validation error result</returns>
    public static ValidationErrorResult FromFailure(ValidationFailure failure)
    {
        return new ValidationErrorResult(failure.ErrorCode, failure.ErrorMessage);
    }
}