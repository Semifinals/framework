using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Semifinals.Framework.Testing;

namespace Semifinals.Framework.Tests.Unit;

[TestClass]
public class FunctionBuilderTests : Test
{
    [TestMethod]
    public void Init_InitializesFunctionBuilder()
    {
        // Arrange

        // Act
        dynamic builder = FunctionBuilder.Init();

        // Assert
        Assert.IsInstanceOfType(builder, typeof(FunctionBuilder<NoBodyDto, NoParamDto>));
    }

    [TestMethod]
    public void AddBody_AddsBodyDto()
    {
        // Arrange
        var builder = FunctionBuilder.Init();

        // Act
        var res = builder.AddBody<TestBodyDto>();

        // Assert
        Assert.IsInstanceOfType(res, typeof(FunctionBuilder<TestBodyDto, NoParamDto>));
    }

    [TestMethod]
    public void AddParams_AddsParamsDto()
    {
        // Arrange
        var builder = FunctionBuilder.Init();

        // Act
        var res = builder.AddParams<TestParamDto>();

        // Assert
        Assert.IsInstanceOfType(res, typeof(FunctionBuilder<NoBodyDto, TestParamDto>));
    }

    [TestMethod]
    public void AddParams_AddsOnTopOfBody()
    {
        // Arrange
        var builder = FunctionBuilder.Init();

        // Act
        var res = builder.AddBody<TestBodyDto>().AddParams<TestParamDto>();

        // Assert
        Assert.IsInstanceOfType(res, typeof(FunctionBuilder<TestBodyDto, TestParamDto>));
    }

    [TestMethod]
    public async Task Build_CreatesFunction()
    {
        // Arrange
        HttpRequest req = await CreateRequest();

        // Act
        dynamic res = null!;
        await FunctionBuilder.Init().Build(req)(async func =>
        {
            res = func;
            await Task.Delay(1);
            return new NoContentResult();
        });

        // Assert
        Assert.IsInstanceOfType(res, typeof(Function<NoBodyDto, NoParamDto>));
    }
}

public class TestBodyDto : Dto, IBodyDto
{
    public string? nonce;

    public override IDtoValidator Validator { get; } = new DtoValidator<TestBodyDto>(validator =>
    {
        validator.RuleFor(x => x.nonce)
            .NotNull()
            .NotEmpty();
    });
}

public class TestParamDto : Dto, IParamDto
{
    public string? nonce;

    public override IDtoValidator Validator { get; } = new DtoValidator<TestParamDto>(validator =>
    {
        validator.RuleFor(x => x.nonce)
            .NotNull()
            .NotEmpty();
    });
}