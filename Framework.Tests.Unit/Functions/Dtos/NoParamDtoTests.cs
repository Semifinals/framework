using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Semifinals.Framework.Tests.Unit;

[TestClass]
public class NoParamDtoTests
{
    [TestMethod]
    public void Test_ValidDto()
    {
        // Arrange
        NoParamDto dto = new();

        // Act
        ValidationResult res = dto.Validator.Test(dto);
        bool valid = res.IsValid;

        // Assert
        Assert.IsTrue(valid);
    }
}