using FluentValidation;
using Newtonsoft.Json;
using Semifinals.Framework.Utils.Exceptions;
using System.Text.RegularExpressions;

namespace Semifinals.Framework.Testing;

/// <summary>
/// DTO for testing the body of an example request.
/// </summary>
public class TestBodyDto : Dto, IBodyDto
{
    [JsonProperty("nonce")]
    public string Nonce { get; set; } = null!;
    
    public override IDtoValidator Validator { get; } = new DtoValidator<TestBodyDto>(validator =>
    {
        validator.RuleFor(x => x.Nonce)
            .Matches(new Regex("nonce"))
            .SetError("F010", "Invalid test nonce");
    });
}

/// <summary>
/// DTO for testing the params of an example request.
/// </summary>
public class TestParamDto : Dto, IParamDto
{
    [JsonProperty("nonce")]
    public string Nonce { get; set; } = null!;

    public override IDtoValidator Validator { get; } = new DtoValidator<TestParamDto>(validator =>
    {
        validator.RuleFor(x => x.Nonce)
            .Matches(new Regex("nonce"))
            .SetError("F010", "Invalid test nonce");
    });
}