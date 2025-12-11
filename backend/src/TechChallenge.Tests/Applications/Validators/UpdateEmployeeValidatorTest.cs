using Bogus;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Commands.UpdateEmployee;
using TechChallenge.Application.Validators;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Tests.Applications.Validators;

public class UpdateEmployeeValidatorTest
{
    private readonly Mock<IEmployeeQueryStore> _employeeQueryStoreMock;
    private readonly UpdateEmployeeValidator _validator;
    private readonly Faker _faker;

    public UpdateEmployeeValidatorTest()
    {
        _employeeQueryStoreMock = new Mock<IEmployeeQueryStore>();
        _validator = new UpdateEmployeeValidator(_employeeQueryStoreMock.Object);
        _faker = new Faker("pt_BR");
    }

    private UpdateEmployeeCommand CreateValidCommand(int employeeId)
    {
        var oldestReferenceDate = DateTime.UtcNow.AddYears(-18).AddDays(-1);
        var youngestReferenceDate = DateTime.UtcNow.AddYears(-25);

        return new UpdateEmployeeCommand(
            EmployeeId: employeeId,
            FirstName: _faker.Person.FirstName,
            LastName: _faker.Person.LastName,
            Email: _faker.Internet.Email(),
            DocumentNumber: _faker.Person.Random.Int(11111, 99999).ToString(),
            BirthDate: _faker.Date.Between(youngestReferenceDate, oldestReferenceDate),
            EmployeeRoleType.User,
            Phones:
            [
                new (PhoneType.Mobile, _faker.Phone.PhoneNumber()),
            ]
        );
    }

    [Fact(DisplayName = "Validator | Update Employee | Should pass validation when command is valid and email is unique")]
    public async Task Should_PassValidation_WhenCommandIsValidAndEmailIsUnique()
    {
        var command = CreateValidCommand(employeeId: 5);

        _employeeQueryStoreMock.Setup(x => x.GetEmailExists(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();

        _employeeQueryStoreMock.Verify(x => x.GetEmailExists(command.Email.Trim()), Times.Once);
    }

    [Theory(DisplayName = "Validator | Update Employee | Should fail when EmployeeId is invalid")]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Should_Fail_WhenEmployeeIdIsInvalid(int invalidId)
    {
        var command = CreateValidCommand(employeeId: invalidId);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Invalid Employee Id.");
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(nameof(UpdateEmployeeCommand.EmployeeId));
        result.Errors.Should().ContainSingle()
              .Which.CustomState.Should().Be(ErrorType.INVALID_INPUT);

        _employeeQueryStoreMock.Verify(x => x.GetEmailExists(It.IsAny<string>()), Times.Never);
    }

    [Theory(DisplayName = "Validator | Update Employee | Should fail when required string fields are missing")]
    [InlineData("", "LastName", "teste@email.com", "First name is required.", nameof(UpdateEmployeeCommand.FirstName))]
    [InlineData("FirstName", "", "teste@email.com", "Last name is required.", nameof(UpdateEmployeeCommand.LastName))]
    [InlineData("FirstName", "LastName", "", "Email is required.", nameof(UpdateEmployeeCommand.Email))]
    public async Task Should_Fail_WhenRequiredFieldsAreMissing(string firstName, string lastName, string email, string expectedMessage, string expectedField)
    {
        var command = CreateValidCommand(employeeId: 10) with
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
        };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be(expectedMessage);
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(expectedField);
        result.Errors.Should().ContainSingle()
              .Which.CustomState.Should().Be(ErrorType.INVALID_INPUT);

        _employeeQueryStoreMock.Verify(x => x.GetEmailExists(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Validator | Update Employee | Should fail when Employee is under 18 years old")]
    public async Task Should_Fail_WhenEmployeeIsUnder18()
    {
        var command = CreateValidCommand(employeeId: 5) with { BirthDate = _faker.Date.Past(10) };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Employee must be older than 18.");
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(nameof(UpdateEmployeeCommand.BirthDate));
    }

    [Fact(DisplayName = "Validator | Update Employee | Should fail when Email format is invalid")]
    public async Task Should_Fail_WhenEmailFormatIsInvalid()
    {
        var command = CreateValidCommand(employeeId: 5) with { Email = "invalid-email-at-sign" };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Invalid email format.");
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(nameof(UpdateEmployeeCommand.Email));

        _employeeQueryStoreMock.Verify(x => x.GetEmailExists(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Validator | Update Employee | Should fail when Email already exists (Duplicate)")]
    public async Task Should_Fail_WhenEmailAlreadyExists()
    {
        var command = CreateValidCommand(employeeId: 5);

        _employeeQueryStoreMock.Setup(x => x.GetEmailExists(It.IsAny<string>())).ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Email is already in use.");
        result.Errors.Should().ContainSingle()
              .Which.CustomState.Should().Be(ErrorType.DUPLICATE_EMAIL);

        _employeeQueryStoreMock.Verify(x => x.GetEmailExists(command.Email.Trim()), Times.Once);
    }
}
