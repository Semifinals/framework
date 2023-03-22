namespace Semifinals.Framework;

public class NoParamDto : Dto, IParamDto
{
    public override IDtoValidator Validator { get; } = new DtoValidator<NoParamDto>(validator => { });
}