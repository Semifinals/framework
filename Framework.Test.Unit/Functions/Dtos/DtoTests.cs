using FluentValidation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Semifinals.Framework.Tests.Unit;

[TestClass]
public class DtoValidatorTests
{
    [TestMethod]
    public void Test_ValidDto()
    {
        // Arrange
        TestDto dto = new()
        {
            valid = true
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);
        bool valid = res.IsValid;

        // Assert
        Assert.IsTrue(valid);
    }

    [TestMethod]
    public void Test_InvalidDto()
    {
        // Arrange
        TestDto dto = new()
        {
            valid = false
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);
        bool valid = res.IsValid;

        // Assert
        Assert.IsFalse(valid);
    }
}

public class TestDto : Dto
{
    public bool valid;

    public override IDtoValidator Validator { get; } = new DtoValidator<TestDto>(validator =>
    {
        validator.RuleFor(x => x.valid).Equal(true);
    });
}