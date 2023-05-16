using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Semifinals.Framework.Testing;
using Semifinals.Framework.Utils.Exceptions;

namespace Framework.Tests.Unit;

[TestClass]
public class ValidationErrorResultTests
{
    [TestMethod]
    public void Constructor_ConstructsValidError()
    {
        // Arrange
        string code = "T000";
        string message = "Test Message";

        // Act
        ValidationErrorResult result = new(code, message);

        // Assert
        Assert.AreEqual(code, result.Code);
        Assert.AreEqual(message, result.Message);
    }

    [TestMethod]
    public void Serialize_SerializesError()
    {
        // Arrange
        string code = "T000";
        string message = "Test Message";
        ValidationErrorResult result = new(code, message);
        string error = @$"{{""code"":""{code}"",""message"":""{message}""}}";

        // Act
        string res = JsonConvert.SerializeObject(result);

        // Assert
        Assert.AreEqual(error, res);
    }

    [TestMethod]
    public void Deserialize_DeserializesError()
    {
        // Arrange
        string code = "T000";
        string message = "Test Message";
        string error = @$"{{""code"":""{code}"",""message"":""{message}""}}";

        // Act
        ValidationErrorResult result = JsonConvert.DeserializeObject<ValidationErrorResult>(error)!;

        // Assert
        Assert.AreEqual(code, result.Code);
        Assert.AreEqual(message, result.Message);
    }

    [TestMethod]
    public void FromFailure_ConvertsToResult()
    {
        // Arrange
        TestBodyDto dto = new()
        {
            Nonce = "invalid"
        };

        // Act
        ValidationResult res = dto.Validator.Test(dto);
        ValidationErrorResult[] errors = res.Errors
            .Select(err => ValidationErrorResult.FromFailure(err))
            .ToArray();

        // Assert
        Assert.AreEqual("F010", errors[0].Code);
    }
}