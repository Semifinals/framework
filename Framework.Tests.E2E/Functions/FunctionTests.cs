using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Semifinals.Framework.Services.Jwt;
using Semifinals.Framework.Testing;

namespace Semifinals.Framework.Tests.E2E;

[TestClass]
public class FunctionTests : Test
{
    [TestMethod]
    public async Task Constructor_BuildsCorrectly()
    {
        // Arrange
        TestBodyDto bodyDto = new()
        {
            nonce = "nonce"
        };
        TestParamDto paramDto = new()
        {
            nonce = "nonce"
        };

        HttpMethod method = HttpMethod.POST;
        string authorizationHeader = "Bearer TOKEN_WOULD_BE_HERE";
        HttpRequest req = await CreateRequest(bodyDto, method, authorizationHeader);

        bool requiresAuth = true;
        int requiresFlags = 1;

        // Act
        Function<TestBodyDto, TestParamDto> func = new(req, bodyDto, paramDto, requiresAuth, requiresFlags, "jwtSecret");

        // Assert
        Assert.AreEqual(requiresAuth, func.RequiresAuth);
        Assert.AreEqual(requiresFlags, func.RequiresFlags);
        Assert.AreEqual(method, func.Method);
        Assert.IsInstanceOfType(func.Body, typeof(TestBodyDto));
        Assert.IsInstanceOfType(func.Params, typeof(TestParamDto));
        Assert.AreEqual(authorizationHeader, func.Headers["Authorization"].First());
    }

    [TestMethod]
    public async Task ValidateBody_AcceptsValid()
    {
        // Arrange
        TestBodyDto dto = new()
        {
            nonce = "nonce"
        };
        HttpRequest req = await CreateRequest(dto);

        // Act
        TestBodyDto res = await Function<TestBodyDto>.ValidateBody(req);

        // Assert
        Assert.IsInstanceOfType(res, typeof(TestBodyDto));
        Assert.AreEqual("nonce", res.nonce);
    }

    [TestMethod]
    public async Task ValidateBody_RejectsInvalid()
    {
        // Arrange
        TestBodyDto dto = new();
        HttpRequest req = await CreateRequest(dto);

        // Act
        async Task<TestBodyDto> res() => await Function<TestBodyDto>.ValidateBody(req);

        // Assert
        await Assert.ThrowsExceptionAsync<ValidationException>(res);
    }

    [TestMethod]
    public async Task ValidateParams_AcceptsValid()
    {
        // Arrange
        QueryBuilder query = new();
        query.Add("nonce", "nonce");

        HttpRequest req = await CreateRequest<NoBodyDto>(new(), query: query);

        // Act
        TestParamDto res = Function<NoBodyDto, TestParamDto>.ValidateParams(req);

        // Assert
        Assert.IsInstanceOfType(res, typeof(TestParamDto));
        Assert.AreEqual("nonce", res.nonce);
    }

    [TestMethod]
    public async Task ValidateParams_UsesFirstOfKey()
    {
        // Arrange
        QueryBuilder query = new();
        query.Add("nonce", "nonce1");
        query.Add("nonce", "nonce2");


        HttpRequest req = await CreateRequest<NoBodyDto>(new(), query: query);

        // Act
        TestParamDto res = Function<NoBodyDto, TestParamDto>.ValidateParams(req);

        // Assert
        Assert.IsInstanceOfType(res, typeof(TestParamDto));
        Assert.AreEqual("nonce1", res.nonce);
    }

    [TestMethod]
    public async Task ValidateParams_RejectsInvalid()
    {
        // Arrange
        QueryBuilder query = new();

        HttpRequest req = await CreateRequest<NoBodyDto>(new(), query: query);

        // Act
        TestParamDto res() => Function<NoBodyDto, TestParamDto>.ValidateParams(req);

        // Assert
        Assert.ThrowsException<ValidationException>(res);
    }

    [TestMethod]
    public async Task VerifyUser_AcceptsNoRequirements()
    {
        // Arrange
        HttpRequest req = await CreateRequest<NoBodyDto>(new());
        Function func = new(req, false, 0, "jwtSecret");

        // Act
        Function.VerifyUser(func);

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task VerifyUser_OnlyAuth_AcceptsValid()
    {
        // Arrange
        string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE2Nzk0NTkzMTE1NDcsImV4cCI6MTg5MzQ1NjAwMDAwMCwic3ViIjoic3ViamVjdCIsIkZsYWdzIjowfQ==.1ZUSmikFsP0nAOa06oDJnWvwOVYfr6ItzEcezjKuOyI=";
        string authorizationHeader = $"Bearer {jwt}";
        HttpRequest req = await CreateRequest<NoBodyDto>(new(), authorizationHeader: authorizationHeader);
        Function func = new(req, true, 0, "jwtSecret");

        // Act
        Function.VerifyUser(func);

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task VerifyUser_OnlyAuth_RejectsInvalid()
    {
        // Arrange
        HttpRequest req = await CreateRequest<NoBodyDto>(new());
        Function func = new(req, true, 0, "jwtSecret");

        // Act
        void res() => Function.VerifyUser(func);

        // Assert
        Assert.ThrowsException<UnauthorizedException>(res);
    }

    [TestMethod]
    public async Task VerifyUser_WithFlags_AcceptsValid()
    {
        // Arrange
        string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE2Nzk0NTkzMTE1NDcsImV4cCI6MTg5MzQ1NjAwMDAwMCwic3ViIjoic3ViamVjdCIsIkZsYWdzIjoxfQ==.hJBt5rpM7jIdWvgrckwl5eh2IxTb1srEoRRzjYwYTLM=";
        string authorizationHeader = $"Bearer {jwt}";
        HttpRequest req = await CreateRequest<NoBodyDto>(new(), authorizationHeader: authorizationHeader);
        Function func = new(req, true, 1, "jwtSecret");

        // Act
        Function.VerifyUser(func);

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task VerifyUser_WithFlags_RejectsInvalid()
    {
        // Arrange
        string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE2Nzk0NTkzMTE1NDcsImV4cCI6MTg5MzQ1NjAwMDAwMCwic3ViIjoic3ViamVjdCIsIkZsYWdzIjowfQ==.1ZUSmikFsP0nAOa06oDJnWvwOVYfr6ItzEcezjKuOyI=";
        string authorizationHeader = $"Bearer {jwt}";
        HttpRequest req = await CreateRequest<NoBodyDto>(new(), authorizationHeader: authorizationHeader);
        Function func = new(req, true, 1, "jwtSecret");

        // Act
        void res() => Function.VerifyUser(func);

        // Assert
        Assert.ThrowsException<ForbiddenException>(res);
    }

    [TestMethod]
    public async Task Run_RunsCallback()
    {
        // Arrange
        HttpRequest req = await CreateRequest<NoBodyDto>(new());
        Function func = new(req, false, 0);

        // Act
        IActionResult res = await Function.Run(req)(async func =>
        {
            await Task.Delay(1);
            return new NoContentResult();
        });

        // Assert
        Assert.IsInstanceOfType(res, typeof(NoContentResult));
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