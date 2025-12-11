using Microsoft.AspNetCore.Mvc;
using TechChallenge.Application.Commands.CreateEmployee;
using TechChallenge.Application.Commands.DeleteEmployee;
using TechChallenge.Application.Commands.Login;
using TechChallenge.Application.Commands.UpdateEmployee;
using TechChallenge.Application.Queries.GetAllEmployes;
using TechChallenge.Application.Queries.GetEmployeeById;

namespace TechChallenge.Api.Extensions;

public static class ResponseExtension
{
    public static IActionResult ToHttpResponse(this CreateEmployeeCommandResult result)
    {
        if (result.ErrorCode == "USER_UNAUTHORIZED")
            return new ForbidResult();

        if (result.Success)
            return new OkObjectResult(result);

        return new BadRequestObjectResult(result);
    }

    public static IActionResult ToHttpResponse(this UpdateEmployeeCommandResult result)
    {
        if (result.ErrorCode == "USER_UNAUTHORIZED")
            return new ForbidResult();

        if (result.Success)
            return new OkObjectResult(result);

        return new BadRequestObjectResult(result);
    }

    public static IActionResult ToHttpResponse(this DeleteEmployeeCommanResult result)
    {
        if (result.ErrorCode == "USER_UNAUTHORIZED")
            return new ForbidResult();

        if (result.Success)
            return new NoContentResult();

        return new BadRequestObjectResult(result);
    }

    public static IActionResult ToHttpResponse(this GetEmployeeByIdCommandResult result)
    {
        if (result.ErrorCode == "USER_UNAUTHORIZED")
            return new ForbidResult();

        if (result.Success)
            return new OkObjectResult(result);

        return new BadRequestObjectResult(result);
    }

    public static IActionResult ToHttpResponse(this GetAllCommandResult result)
    {
        if (result.ErrorCode == "USER_UNAUTHORIZED")
            return new ForbidResult();

        if (result.Success)
            return new OkObjectResult(result);

        return new BadRequestObjectResult(result);
    }
    
    public static IActionResult ToHttpResponse(this LoginCommandResult result)
    {
        if (result.ErrorCode == "USER_UNAUTHORIZED")
            return new ForbidResult();

        if (result.Success)
            return new OkObjectResult(result);

        return new BadRequestObjectResult(result);
    }
}
