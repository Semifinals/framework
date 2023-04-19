﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace Semifinals.Framework.Demo;

public class TestController
{
    [FunctionName("Ping")]
    public static async Task<IActionResult> Ping(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ping")] HttpRequest req)
    {
        return await FunctionBuilder.Init().Build(req)(async func =>
        {
            await Task.Delay(1);
            return new OkObjectResult("Pong!");
        });
    }

    [FunctionName("Body")]
    public static async Task<IActionResult> Body(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "body")] HttpRequest req)
    {
        return await FunctionBuilder.Init()
            .AddBody<TestDto>()
            .Build(req)(async func =>
        {
            await Task.Delay(1);
            return new OkObjectResult("Pong!");
        });
    }
}

public class TestDto : Dto, IBodyDto
{
    public bool valid;

    public override IDtoValidator Validator { get; } = new DtoValidator<TestDto>(validator =>
    {
        validator.RuleFor(x => x.valid).Equal(true);
    });
}