using FluentValidation;
using FluentValidation.Results;
using System.Net.Mail;
using TechChallenge.Application.Commands.UpdateEmployee;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Validators;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeValidator(IEmployeeQueryStore employeeQueryStore)
    {
        RuleFor(x => x).Custom(async (command, context) =>
        {
            if (command.EmployeeId <= 0)
            {
                context.AddFailure(new ValidationFailure(nameof(command.EmployeeId), "Invalid Employee Id.")
                {
                    CustomState = ErrorType.INVALID_INPUT
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(command.FirstName))
            {
                context.AddFailure(new ValidationFailure(nameof(command.FirstName), "First name is required.")
                {
                    CustomState = ErrorType.INVALID_INPUT
                });

                return;
            }

            if (string.IsNullOrWhiteSpace(command.LastName))
            {
                context.AddFailure(new ValidationFailure(nameof(command.LastName), "Last name is required.")
                {
                    CustomState = ErrorType.INVALID_INPUT
                });

                return;
            }

            if (command.BirthDate > DateTime.UtcNow.AddYears(-18))
            {
                context.AddFailure(new ValidationFailure(nameof(command.BirthDate), "Employee must be older than 18.")
                {
                    CustomState = ErrorType.INVALID_INPUT
                });

                return;
            }

            if (string.IsNullOrWhiteSpace(command.Email))
            {
                context.AddFailure(new ValidationFailure(nameof(command.Email), "Email is required.")
                {
                    CustomState = ErrorType.INVALID_INPUT
                });

                return;
            }
            else
            {
                var email = command.Email.Trim();
                try
                {
                    var _ = new MailAddress(email);

                    if (await employeeQueryStore.GetEmailExists(email))
                        context.AddFailure(new ValidationFailure(nameof(command.Email), "Email is already in use.")
                        {
                            CustomState = ErrorType.DUPLICATE_EMAIL
                        });

                    return;
                }
                catch
                {
                    context.AddFailure(new ValidationFailure(nameof(command.Email), "Invalid email format.")
                    {
                        CustomState = ErrorType.INVALID_INPUT
                    });
                }
            }
        });
    }
}
