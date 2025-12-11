using MediatR;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler(IEmployeeDomainService employeeDomainService, IEmployeeCommandStore employeeCommandStore) 
    : IRequestHandler<UpdateEmployeeCommand, UpdateEmployeeCommandResult>
{
    private readonly IEmployeeDomainService _employeeDomainService = employeeDomainService;
    private readonly IEmployeeCommandStore _employeeCommandStore = employeeCommandStore;

    public async Task<UpdateEmployeeCommandResult> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await _employeeDomainService.UpdateEmployee(
            command.EmployeeId, 
            command.FirstName, 
            command.LastName, 
            command.Email, 
            command.BirthDate,
            command.Phones.ToDictionary(phone => phone.Number, phone => phone.Type), 
            command.DocumentNumber,
            command.AuthRole
        );

        await _employeeCommandStore.UpdateAsync(employee);

        return UpdateEmployeeCommandResult.Ok();
    }
}
