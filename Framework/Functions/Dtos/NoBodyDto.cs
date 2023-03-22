namespace Semifinals.Framework;

public class NoBodyDto : Dto, IBodyDto
{
    public override IDtoValidator Validator { get; } = new DtoValidator<NoBodyDto>(validator => { });
}