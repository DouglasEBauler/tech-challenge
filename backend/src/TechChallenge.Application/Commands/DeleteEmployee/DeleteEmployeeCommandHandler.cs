using MediatR;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler(IEmployeeDomainService employeeDomainService, IEmployeeCommandStore employeeCommandStore)
    : IRequestHandler<DeleteEmployeeCommand, DeleteEmployeeCommanResult>
{
    private readonly IEmployeeDomainService _employeeDomainService = employeeDomainService;
    private readonly IEmployeeCommandStore _employeeCommandStore = employeeCommandStore;

    public async Task<DeleteEmployeeCommanResult> Handle(DeleteEmployeeCommand command, CancellationToken ct)
    {
        var employee = await _employeeDomainService.DeleteEmployeeAsync(command.EmployeeId, command.AuthRole);

        await _employeeCommandStore.DeleteAsync(employee);

        return DeleteEmployeeCommanResult.Ok();
    }
}
