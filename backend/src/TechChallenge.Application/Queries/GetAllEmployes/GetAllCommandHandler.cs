using MediatR;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Queries.GetAllEmployes;

public class GetAllCommandHandler(IEmployeeDomainService employeeDomainService)
    : IRequestHandler<GetAllCommand, GetAllCommandResult>
{
    private readonly IEmployeeDomainService _employeeDomainService = employeeDomainService;

    public async Task<GetAllCommandResult> Handle(GetAllCommand command, CancellationToken cancellationToken)
    {
        var employes = await _employeeDomainService.GetAllAsync(command.AuthRole, command.AuthEmployeeId);
        var getCommandResults = new List<GetAllEmployeeCommandResult>();

        foreach (var employee in employes)
        {
            getCommandResults.Add(new GetAllEmployeeCommandResult(
                employee.Id,
                employee.FirstName,
                employee.LastName, 
                employee.Email, 
                employee.BirthDate, 
                [.. employee.Phones.Select(p => p.Number)],
                employee.Manager?.FirstName + employee.Manager?.LastName,
                employee.Role
            ));
        }

        return GetAllCommandResult.Ok(getCommandResults);
    }
}
