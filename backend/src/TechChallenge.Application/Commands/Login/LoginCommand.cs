using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Commands.Login;

public record LoginCommand(string Email, string Password)
    : ICommand<LoginCommandResult>;