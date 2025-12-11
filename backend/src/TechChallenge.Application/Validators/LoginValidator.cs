using FluentValidation;
using FluentValidation.Results;
using TechChallenge.Application.Commands.Login;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Validators;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x).Custom((command, context) =>
        {
            if (string.IsNullOrWhiteSpace(command.Email))
            {
                context.AddFailure(new ValidationFailure(nameof(command.Email), "Email is required.")
                {
                    CustomState = ErrorType.INVALID_INPUT
                });

                return;
            }

            if (string.IsNullOrWhiteSpace(command.Password))
            {
                context.AddFailure(new ValidationFailure(nameof(command.Password), "Password is required.")
                {
                    CustomState = ErrorType.INVALID_INPUT
                });

                return;
            }
        });
    }
}
