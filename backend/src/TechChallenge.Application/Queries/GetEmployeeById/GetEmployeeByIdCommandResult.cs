using TechChallenge.Application.Results;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Queries.GetEmployeeById;

public record GetEmployeeByIdCommandResult(bool Success, int EmployeeId, string FirstName, string LastName, string Email, string DocumentNumber, DateTime BirthDate, EmployeeRoleType Role, List<GetEmployeePhoneByIdCommandResult> Phones, string? ErrorCode = null, string? ErrorMessage = null) 
    : ICommandResult
{
    public bool Success { get; set; } = Success;
    public string? ErrorCode { get; set; } = ErrorCode;
    public string? ErrorMessage { get; set; } = ErrorMessage;

    public static GetEmployeeByIdCommandResult Ok(int employeeId, string firstName, string lastName, string email, string documentNumber, DateTime birthDate, EmployeeRoleType employeeRoleType, List<GetEmployeePhoneByIdCommandResult> phones)
        => new(true, employeeId, firstName, lastName, email, documentNumber, birthDate, employeeRoleType, phones);

    public static CommandResult Fail(ErrorType errorCode, string errorMessage)
        => new(false, errorCode.ToString(), errorMessage);
}