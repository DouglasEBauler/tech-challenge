using TechChallenge.Application.Results;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.DeleteEmployee;

public record DeleteEmployeeCommanResult(bool Success, string? ErrorCode = null, string? ErrorMessage = null) 
    : ICommandResult
{
    public bool Success { get; set; } = Success;
    public string? ErrorCode { get; set; } = ErrorCode;
    public string? ErrorMessage { get; set; } = ErrorMessage;

    public static DeleteEmployeeCommanResult Ok()
        => new(true);

    public static CommandResult Fail(ErrorType errorCode, string errorMessage)
        => new(false, errorCode.ToString(), errorMessage);
}
