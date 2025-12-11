using TechChallenge.Domain.Enums;

namespace TechChallenge.Application.Commands.CreateEmployee;

public record CreateEmployeePhoneCommand(PhoneType Type, string Number);