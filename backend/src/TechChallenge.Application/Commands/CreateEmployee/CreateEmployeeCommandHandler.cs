using MediatR;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(IEmployeeDomainService employeeDomainService, IEmployeeCommandStore employeeCommandStore) 
    : IRequestHandler<CreateEmployeeCommand, CreateEmployeeCommandResult>
{
    private readonly IEmployeeDomainService _employeeDomainService = employeeDomainService;
    private readonly IEmployeeCommandStore _employeeCommandStore = employeeCommandStore;

    public async Task<CreateEmployeeCommandResult> Handle(CreateEmployeeCommand command, CancellationToken ct)
    {
        var employee = _employeeDomainService.CreateEmployee(
            command.FirstName, 
            command.LastName,
            command.Email,
            command.DocumentNumber, 
            command.Password, 
            command.BirthDate, 
            command.Role,
            command.Phones.ToDictionary(phone => phone.Number, phone => phone.Type),
            command.AuthRole,
            command.AuthEmployeeId
        );

        await _employeeCommandStore.AddAsync(employee);

        return CreateEmployeeCommandResult.Ok(employee.Id);
    }
}
