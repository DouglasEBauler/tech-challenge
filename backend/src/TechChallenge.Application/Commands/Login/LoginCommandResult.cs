using TechChallenge.Application.Commands.UpdateEmployee;
using TechChallenge.Application.Results;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.Login;

public class LoginCommandResult(bool Success, string token, string? ErrorCode = null, string? ErrorMessage = null) 
    : ICommandResult
{
    public bool Success { get; set; } = Success;
    public string? ErrorCode { get; set; } = ErrorCode;
    public string? ErrorMessage { get; set; } = ErrorMessage;
    
    public string Token { get; set; } = token;

    public static LoginCommandResult Ok(string token)
        => new(true, token);

    public static CommandResult Fail(ErrorType errorCode, string errorMessage)
        => new(false, errorCode.ToString(), errorMessage);
}