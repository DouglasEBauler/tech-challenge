using Bogus;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Commands.CreateEmployee;
using TechChallenge.Application.Validators;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Helpers;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Tests.Applications.Validators;

public class CreateEmployeeValidatorTest
{
    private readonly Mock<IEmployeeQueryStore> _employeeQueryStoreMock;
    private readonly CreateEmployeeValidator _validator;
    private readonly Faker _faker;

    public CreateEmployeeValidatorTest()
    {
        _employeeQueryStoreMock = new Mock<IEmployeeQueryStore>();
        _validator = new CreateEmployeeValidator(_employeeQueryStoreMock.Object);
        _faker = new Faker("pt_BR");
    }

    private CreateEmployeeCommand CreateValidCommand()
    {
        return new CreateEmployeeCommand(
            FirstName: _faker.Person.FirstName,
            LastName: _faker.Person.LastName,
            Email: _faker.Internet.Email(),
            DocumentNumber: _faker.Person.Random.Int(11111, 99999).ToString(),
            BirthDate: _faker.Date.Past(40, DateTime.UtcNow.AddYears(-20)),
            Phones:
            [
                new (PhoneType.Mobile, _faker.Phone.PhoneNumber()),
            ],
            EmployeeRoleType.User,
            Password: "!Password123"
        );
    }

    [Fact(DisplayName = "Validator | Create Employee | Should pass validation when command is valid and unique")]
    public async Task Should_PassValidation_WhenCommandIsValidAndUnique()
    {
        var command = CreateValidCommand();

        _employeeQueryStoreMock.Setup(x => x.GetDocumentNumberExists(It.IsAny<string>())).ReturnsAsync(false);
        _employeeQueryStoreMock.Setup(x => x.GetEmailExists(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();

        _employeeQueryStoreMock.Verify(
            x => x.GetDocumentNumberExists(EncryptionHelper.CreateIndexHash(command.DocumentNumber)),
            Times.Once);
        _employeeQueryStoreMock.Verify(x => x.GetEmailExists(command.Email.Trim()), Times.Once);
    }

    [Theory(DisplayName = "Validator | Create Employee | Should fail when required fields are missing")]
    [InlineData("", "LastName", "Email", "DocumentNumber", "First name is required.", nameof(CreateEmployeeCommand.FirstName))]
    [InlineData("FirstName", "", "Email", "DocumentNumber", "Last name is required.", nameof(CreateEmployeeCommand.LastName))]
    [InlineData("FirstName", "LastName", "Email", "", "Document number is required.", nameof(CreateEmployeeCommand.DocumentNumber))]
    [InlineData("FirstName", "LastName", "", "DocumentNumber", "Email is required.", nameof(CreateEmployeeCommand.Email))]
    public async Task Should_Fail_WhenRequiredFieldsAreMissing(
        string firstName, string lastName, string email, string documentNumber, string expectedMessage, string expectedField)
    {
        var command = CreateValidCommand() with
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DocumentNumber = documentNumber,
            Phones = []
        };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be(expectedMessage);
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(expectedField);
        result.Errors.Should().ContainSingle()
              .Which.CustomState.Should().Be(ErrorType.INVALID_INPUT);
    }

    [Fact(DisplayName = "Validator | Create Employee | Should fail when Password is missing")]
    public async Task Should_Fail_WhenPasswordIsMissing()
    {
        var command = CreateValidCommand() with { Password = null! };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Password is required.");
    }

    [Fact(DisplayName = "Validator | Create Employee | Should fail when Phones list is missing")]
    public async Task Should_Fail_WhenPhonesListIsMissing()
    {
        var command = CreateValidCommand() with { Phones = null! };

        _employeeQueryStoreMock.Setup(x => x.GetDocumentNumberExists(It.IsAny<string>())).ReturnsAsync(false);
        _employeeQueryStoreMock.Setup(x => x.GetEmailExists(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("At least one phone is required.");
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(nameof(CreateEmployeeCommand.Phones));
    }

    [Fact(DisplayName = "Validator | Create Employee | Should fail when DocumentNumber already exists")]
    public async Task Should_Fail_WhenDocumentNumberAlreadyExists()
    {
        var command = CreateValidCommand();

        _employeeQueryStoreMock.Setup(x => x.GetDocumentNumberExists(It.IsAny<string>())).ReturnsAsync(true);
        _employeeQueryStoreMock.Setup(x => x.GetEmailExists(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Document number is already in use.");
        result.Errors.Should().ContainSingle()
              .Which.CustomState.Should().Be(ErrorType.DUPLICATE_DOCUMENT_NUMBER);

        _employeeQueryStoreMock.Verify(x => x.GetDocumentNumberExists(It.IsAny<string>()), Times.Once);
        _employeeQueryStoreMock.Verify(x => x.GetEmailExists(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Validator | Create Employee | Should fail when Email already exists")]
    public async Task Should_Fail_WhenEmailAlreadyExists()
    {
        var command = CreateValidCommand();

        _employeeQueryStoreMock.Setup(x => x.GetDocumentNumberExists(It.IsAny<string>())).ReturnsAsync(false);
        _employeeQueryStoreMock.Setup(x => x.GetEmailExists(It.IsAny<string>())).ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Email is already in use.");
        result.Errors.Should().ContainSingle()
              .Which.CustomState.Should().Be(ErrorType.DUPLICATE_EMAIL);

        _employeeQueryStoreMock.Verify(x => x.GetDocumentNumberExists(It.IsAny<string>()), Times.Once);
        _employeeQueryStoreMock.Verify(x => x.GetEmailExists(command.Email.Trim()), Times.Once);
    }

    [Theory(DisplayName = "Validator | Create Employee | Should fail when Password requirements are not met")]
    [InlineData("password123", "Password must contain at least one uppercase letter.")]
    [InlineData("PASSWORDDDD", "Password must contain at least one number.")]
    [InlineData("Pass1", "Password must have at least 8 characters.")]
    public async Task Should_Fail_WhenPasswordRequirementsAreNotMet(string password, string expectedMessage)
    {
        var command = CreateValidCommand() with { Password = password };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be(expectedMessage);
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(nameof(CreateEmployeeCommand.Password));
    }

    [Fact(DisplayName = "Validator | Create Employee | Should fail when Employee is under 18 years old")]
    public async Task Should_Fail_WhenEmployeeIsUnder18()
    {
        var command = CreateValidCommand() with { BirthDate = _faker.Date.Past(10) };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Employee must be older than 18.");
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(nameof(CreateEmployeeCommand.BirthDate));
    }

    [Fact(DisplayName = "Validator | Create Employee | Should fail when Email format is invalid")]
    public async Task Should_Fail_WhenEmailFormatIsInvalid()
    {
        var command = CreateValidCommand() with { Email = "invalid-email-at-sign" };

        _employeeQueryStoreMock.Setup(x => x.GetDocumentNumberExists(It.IsAny<string>())).ReturnsAsync(false);
        _employeeQueryStoreMock.Setup(x => x.GetEmailExists(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be("Invalid email format.");
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(nameof(CreateEmployeeCommand.Email));
    }
}
