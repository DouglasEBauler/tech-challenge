using MediatR;
using TechChallenge.Domain.Interfaces;
using TechChallenge.JwtService;

namespace TechChallenge.Application.Commands.Login;

public class LoginCommandHandler(IEmployeeDomainService employeeDomainService, IJwtSecurityService jwtService) 
    : IRequestHandler<LoginCommand, LoginCommandResult>
{
    private readonly IEmployeeDomainService _employeeDomainService = employeeDomainService;
    private readonly IJwtSecurityService _jwtService = jwtService;

    public async Task<LoginCommandResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        return LoginCommandResult.Ok(
            _jwtService.GenerateToken(await _employeeDomainService.LoginEmployeeAsync(command.Email, command.Password)));
    }
}
