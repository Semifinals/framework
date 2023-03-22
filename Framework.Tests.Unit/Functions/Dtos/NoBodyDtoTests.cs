using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Semifinals.Framework.Tests.Unit;

[TestClass]
public class NoBodyDtoTests
{
    [TestMethod]
    public void Test_ValidDto()
    {
        // Arrange
        NoBodyDto dto = new();

        // Act
        ValidationResult res = dto.Validator.Test(dto);
        bool valid = res.IsValid;

        // Assert
        Assert.IsTrue(valid);
    }
}