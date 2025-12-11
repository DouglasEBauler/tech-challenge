using Bogus;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Commands.DeleteEmployee;
using TechChallenge.Application.Validators;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Tests.Applications.Validators;

public class DeleteEmployeeValidatorTest
{
    private readonly Mock<IEmployeeQueryStore> _employeeQueryStoreMock;
    private readonly DeleteEmployeeValidator _validator;
    private readonly Faker _faker;

    public DeleteEmployeeValidatorTest()
    {
        _employeeQueryStoreMock = new Mock<IEmployeeQueryStore>();
        _validator = new DeleteEmployeeValidator(_employeeQueryStoreMock.Object);
        _faker = new Faker("pt_BR");
    }

    private static DeleteEmployeeCommand CreateCommand(int employeeId)
        => new(employeeId);

    [Fact(DisplayName = "Validator | Delete Employee | Should pass validation when EmployeeId is valid and exists")]
    public async Task Should_PassValidation_WhenEmployeeIdIsValidAndExists()
    {
        var employeeId = _faker.Random.Int(1, 100);
        var command = CreateCommand(employeeId);

        _employeeQueryStoreMock.Setup(x => x.GetEmployeeExistsById(employeeId)).ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();

        _employeeQueryStoreMock.Verify(x => x.GetEmployeeExistsById(employeeId), Times.Once);
    }

    [Theory(DisplayName = "Validator | Delete Employee | Should fail when EmployeeId is invalid (<= 0)")]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Should_Fail_WhenEmployeeIdIsInvalid(int invalidId)
    {
        var command = CreateCommand(invalidId);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Invalid Employee Id.");
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(nameof(DeleteEmployeeCommand.EmployeeId));
        result.Errors.Should().ContainSingle()
              .Which.CustomState.Should().Be(ErrorType.INVALID_INPUT);

        _employeeQueryStoreMock.Verify(x => x.GetEmployeeExistsById(It.IsAny<int>()), Times.Never);
    }

    [Fact(DisplayName = "Validator | Delete Employee | Should fail when Employee does not exist")]
    public async Task Should_Fail_WhenEmployeeDoesNotExist()
    {
        var employeeId = 999;
        var command = CreateCommand(employeeId);

        _employeeQueryStoreMock.Setup(x => x.GetEmployeeExistsById(employeeId)).ReturnsAsync(false);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Employee not found.");
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(nameof(DeleteEmployeeCommand.EmployeeId));
        result.Errors.Should().ContainSingle()
              .Which.CustomState.Should().Be(ErrorType.EMPLOYEE_NOT_FOUND);

        _employeeQueryStoreMock.Verify(x => x.GetEmployeeExistsById(employeeId), Times.Once);
    }
}
