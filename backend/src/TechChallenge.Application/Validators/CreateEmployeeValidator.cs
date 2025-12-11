using FluentValidation;
using FluentValidation.Results;
using System.Net.Mail;
using TechChallenge.Application.Commands.CreateEmployee;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Helpers;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Validators;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeValidator(IEmployeeQueryStore employeeQueryStore)
    {
        RuleFor(x => x).Custom(async (command, context) =>
        {
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

            if (string.IsNullOrWhiteSpace(command.DocumentNumber))
            {
                context.AddFailure(new ValidationFailure(nameof(command.DocumentNumber), "Document number is required.")
                {
                    CustomState = ErrorType.INVALID_INPUT
                });

                return;
            }
            else if (await employeeQueryStore.GetDocumentNumberExists(EncryptionHelper.CreateIndexHash(command.DocumentNumber)))
            {
                context.AddFailure(new ValidationFailure(nameof(command.DocumentNumber), "Document number is already in use.")
                {
                    CustomState = ErrorType.DUPLICATE_DOCUMENT_NUMBER
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
            else
            {
                if (command.Password.Length < 8)
                {
                    context.AddFailure(new ValidationFailure(nameof(command.Password), "Password must have at least 8 characters.")
                    {
                        CustomState = ErrorType.INVALID_INPUT
                    });

                    return;
                }

                if (!command.Password.Any(char.IsUpper))
                {
                    context.AddFailure(new ValidationFailure(nameof(command.Password), "Password must contain at least one uppercase letter.")
                    {
                        CustomState = ErrorType.INVALID_INPUT
                    });

                    return;
                }

                if (!command.Password.Any(char.IsDigit))
                {
                    context.AddFailure(new ValidationFailure(nameof(command.Password), "Password must contain at least one number.")
                    {
                        CustomState = ErrorType.INVALID_INPUT
                    });

                    return;
                }
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
                    {
                        context.AddFailure(new ValidationFailure(nameof(command.Email), "Email is already in use.")
                        {
                            CustomState = ErrorType.DUPLICATE_EMAIL
                        });

                        return;
                    }
                }
                catch
                {
                    context.AddFailure(new ValidationFailure(nameof(command.Email), "Invalid email format.")
                    {
                        CustomState = ErrorType.INVALID_INPUT
                    });
                }
            }
            
            if (command.Phones == null || command.Phones.Count == 0)
            {
                context.AddFailure(new ValidationFailure(nameof(command.Phones), "At least one phone is required.")
                {
                    CustomState = ErrorType.INVALID_INPUT
                });

                return;
            }
        });
    }
}
