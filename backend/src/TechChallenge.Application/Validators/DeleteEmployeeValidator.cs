using FluentValidation;
using FluentValidation.Results;
using TechChallenge.Application.Commands.DeleteEmployee;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Validators;

public class DeleteEmployeeValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeValidator(IEmployeeQueryStore employeeQueryStore)
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

            if (!await employeeQueryStore.GetEmployeeExistsById(command.EmployeeId))
            {
                context.AddFailure(new ValidationFailure(nameof(command.EmployeeId), "Employee not found.")
                {
                    CustomState = ErrorType.EMPLOYEE_NOT_FOUND
                });

                return;
            }
        });
    }
}
