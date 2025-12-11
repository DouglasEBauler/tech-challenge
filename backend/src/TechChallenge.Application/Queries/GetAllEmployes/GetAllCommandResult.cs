using TechChallenge.Application.Commands.DeleteEmployee;
using TechChallenge.Application.Results;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Queries.GetAllEmployes;

public record GetAllCommandResult(bool Success, List<GetAllEmployeeCommandResult> Employees, string? ErrorCode = null, string? ErrorMessage = null)
    : ICommandResult
{
    public bool Success { get; set; } = Success;
    public string? ErrorCode { get; set; } = ErrorCode;
    public string? ErrorMessage { get; set; } = ErrorMessage;

    public static GetAllCommandResult Ok(List<GetAllEmployeeCommandResult> getCommandResults)
        => new(true, getCommandResults);

    public static CommandResult Fail(ErrorType errorCode, string errorMessage)
        => new(false, errorCode.ToString(), errorMessage);

}