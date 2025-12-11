using MediatR;
using TechChallenge.Domain.Helpers;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Queries.GetEmployeeById;

public class GetEmployeeByIdCommandHandler(IEmployeeDomainService employeeDomainService)
    : IRequestHandler<GetEmployeeByIdCommand, GetEmployeeByIdCommandResult>
{
    private readonly IEmployeeDomainService _employeeDomainService = employeeDomainService;

    public async Task<GetEmployeeByIdCommandResult> Handle(GetEmployeeByIdCommand command, CancellationToken cancellationToken)
    {
        var employee = await _employeeDomainService.GetByIdAsync(command.Id, command.AuthRole);

        return GetEmployeeByIdCommandResult.Ok(
            employee.Id, 
            employee.FirstName, 
            employee.LastName, 
            employee.Email,
            EncryptionHelper.DecryptDocumentNumber(employee.DocumentNumber), 
            employee.BirthDate, 
            employee.Role,
            [.. employee.Phones.Select(p => new GetEmployeePhoneByIdCommandResult(p.Type.ToString(), p.Number))]
        );
    }
}
