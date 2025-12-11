using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Commands.UpdateEmployee;

public record UpdateEmployeePhoneCommand(PhoneType Type, string Number);
