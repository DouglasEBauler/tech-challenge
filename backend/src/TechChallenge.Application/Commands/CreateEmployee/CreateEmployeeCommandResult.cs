using TechChallenge.Application.Results;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.CreateEmployee;

public record CreateEmployeeCommandResult(bool Success, int? EmployeeId = null, string? ErrorCode = null, string? ErrorMessage = null) 
    : ICommandResult
{
    public bool Success { get; set; } = Success;
    public string? ErrorCode { get; set; } = ErrorCode;
    public string? ErrorMessage { get; set; } = ErrorMessage;

    public static CreateEmployeeCommandResult Ok(int employeeId)
        => new(true, employeeId);

    public static CommandResult Fail(ErrorType errorCode, string errorMessage)
        => new(false, errorCode.ToString(), errorMessage);
}
