using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Api.Attributes;
using TechChallenge.Api.Extensions;
using TechChallenge.Application.Commands.CreateEmployee;
using TechChallenge.Application.Commands.DeleteEmployee;
using TechChallenge.Application.Commands.UpdateEmployee;
using TechChallenge.Application.Queries.GetAllEmployes;
using TechChallenge.Application.Queries.GetEmployeeById;

namespace TechChallenge.Api.Controllers;

/// <summary>
/// Provides API endpoints for employee operations.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/employee")]
[ApiVersion("1.0")]
public class EmployeeController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create new employee.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [Authorize]
    [ApiResponseCreated(typeof(CreateEmployeeCommandResult))]
    [ApiResponseBadRequest]
    public async Task<IActionResult> Create(CreateEmployeeCommand command)
    {
        var result = await _mediator.Send(command);

        return result.ToHttpResponse();
    }

    /// <summary>
    /// Update employee.
    /// </summary>
    /// <param name="id">Id employee update</param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:int}/update")]
    [Authorize]
    [ApiResponseNoContent]
    [ApiResponseBadRequest]
    public async Task<IActionResult> Update(int id, UpdateEmployeeCommand command)
    {
        command = command with { EmployeeId = id };

        var result = await _mediator.Send(command);

        return result.ToHttpResponse();
    }

    /// <summary>
    /// Remove employee.
    /// </summary>
    /// <param name="id">Id employee delete</param>
    /// <returns></returns>
    [HttpDelete("{id:int}/delete")]
    [Authorize]
    [ApiResponseNoContent]
    [ApiResponseBadRequest]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteEmployeeCommand(id));

        return result.ToHttpResponse();
    }

    /// <summary>
    /// Get by Id employee.
    /// </summary>
    /// <param name="id">Id employee</param>
    /// <returns></returns>
    [HttpGet("{id:int}/get")]
    [Authorize]
    [ApiResponseOk(typeof(GetEmployeeByIdCommandResult))]
    [ApiResponseBadRequest]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetEmployeeByIdCommand(id));

        return result.ToHttpResponse();
    }

    /// <summary>
    /// Get all employees.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [ApiResponseOk(typeof(GetAllCommandResult))]
    [ApiResponseBadRequest]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllCommand());

        return result.ToHttpResponse();
    }
}
